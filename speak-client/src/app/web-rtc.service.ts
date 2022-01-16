import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { UserConnection } from "./user-connection";
import { BehaviorSubject } from "rxjs";

@Injectable({ providedIn: 'root' })
export class WebRtcService {

  rtcConfiguration: RTCConfiguration = {
    iceServers: [{ urls: ['stun:stun1.l.google.com:19302', 'stun:stun2.l.google.com:19302'] }],
    iceCandidatePoolSize: 10
  };

  localStream?: MediaStream;

  private currentUserHubConnection?: HubConnection;
  private currentUserId?: string;

  private connections: UserConnection[] = [];
  private connectionsBehaviorSubject = new BehaviorSubject<UserConnection[]>([]);
  public connectionsObservable = this.connectionsBehaviorSubject.asObservable();

  constructor() {
    console.log("Инициализация WebRtcService");
    this.currentUserHubConnection = new HubConnectionBuilder()
      //.withUrl('https://192.168.0.106:443/signallingHub')
      .withUrl('https://facetoface.tech/signallingHub')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Debug)
      .build();

    // Бэк вызывает этот метод у всех других клиентов, когда пользователь оповещает всех о своем подключении с помощью метода "NotifyOthersAboutNewUser"
    this.currentUserHubConnection.on("SendOfferToUser", async (toUserConnectionId: string) =>
      this.sendOfferToUser(toUserConnectionId));

    // Бэкенд вызовет этот метод при отправке текущему юзеру оффера на подключение
    this.currentUserHubConnection.on("ReceiveOffer", async (offer: string, fromUserConnectionId: string) =>
      await this.receiveOfferFromUser(fromUserConnectionId, offer));

    // Этот метод будет вызван с бэка при отправке текущему юзеру ответа на оффер
    this.currentUserHubConnection!.on("ReceiveAnswer", async (answer: string, fromUserId: string) =>
      await this.receiveAnswerFromUser(fromUserId, answer));

    // Метод будет вызван с бэка при отправке текущему юзеру ICE-кандидатов
    this.currentUserHubConnection!.on("ReceiveIceCandidate", async (iceCandidate: string, fromUserId: string) =>
      await this.receiveIceCandidateFromUser(fromUserId, iceCandidate));

    // Этот метод вызывается с бэка, когда пользователь отключается
    this.currentUserHubConnection!.on("DisconnectUser", async (userId: string) =>
      await this.disconnectUser(userId));
  }

  /// Метод подключения к Signalling-серверу
  async startConnection(): Promise<void>{
    if(this.currentUserHubConnection?.state === 'Connected') return;

    await this.currentUserHubConnection!.start();
    console.log('Успешно подключились к Signalling-серверу');
    this.currentUserId = await this.currentUserHubConnection!.invoke<string>("GetConnectionId");
  }

  /// Метод подключения пользователя к созвону. Уведомляет остальных пользователей, провоцируя обмен офферами
  async joinCall(): Promise<void> {
    let otherConnectedUserIds = await this.currentUserHubConnection?.invoke<string[]>("GetOtherConnectedUsers");
    console.log("Получен список людей в комнате: " + otherConnectedUserIds);

    if(otherConnectedUserIds?.length === 0) return;

    for (let userId of otherConnectedUserIds!)
    {
      console.log("Создаем подключение с пользователем: " + userId);
      await this.findOrCreateUserConnectionByUserId(userId);
    }

    await this.currentUserHubConnection?.invoke("NotifyOthersAboutNewUser");
  }

  // Метод отправки оффера другому пользователю
  private async sendOfferToUser(toUserId: string): Promise<void> {
    let connectionWithUser = await this.findOrCreateUserConnectionByUserId(toUserId);

    this.localStream?.getTracks().forEach(track => connectionWithUser.rtcConnection.addTrack(track, this.localStream!));

    let offer = await connectionWithUser.rtcConnection.createOffer();
    await connectionWithUser.rtcConnection.setLocalDescription(offer);

    await this.registerIceCandidatesEventHandler(connectionWithUser.rtcConnection, toUserId);

    await this.currentUserHubConnection!.invoke("SendOffer", toUserId, JSON.stringify(offer));
  }

  // Метод получения оффера от другого пользователя
  private async receiveOfferFromUser(fromUserConnectionId: string, offer: string) {
    console.log("Получен оффер от: " + fromUserConnectionId);

    let connectionWithUser = await this.findOrCreateUserConnectionByUserId(fromUserConnectionId);

    this.localStream?.getTracks().forEach(track => connectionWithUser.rtcConnection.addTrack(track, this.localStream!));

    const description: RTCSessionDescriptionInit = JSON.parse(offer);
    await connectionWithUser.rtcConnection.setRemoteDescription(description);

    const answer = await connectionWithUser.rtcConnection.createAnswer();
    await connectionWithUser.rtcConnection.setLocalDescription(answer);

    await this.registerIceCandidatesEventHandler(connectionWithUser.rtcConnection, fromUserConnectionId);

    await this.currentUserHubConnection!
      .invoke("SendAnswerToUser", connectionWithUser.otherUserHubConnectionId, JSON.stringify(answer));
  }

  // Метод получения ответа от пользователя
  private async receiveAnswerFromUser(fromUserId: string, answer: string) {
    console.log("Получен ответ от " + fromUserId);

    let userConnection = await this.findOrCreateUserConnectionByUserId(fromUserId);

    const answerDescription: RTCSessionDescriptionInit = JSON.parse(answer);
    await userConnection.rtcConnection.setRemoteDescription(answerDescription);

    await this.registerIceCandidatesEventHandler(userConnection.rtcConnection, fromUserId);
  }

  // Метод получения ICE-кандидатов от другого пользователя
  private async receiveIceCandidateFromUser(fromUserId: string, iceCandidate: string) {
    console.log('WebRTC: получен новый ICE candidate');

    let userConnection = await this.findOrCreateUserConnectionByUserId(fromUserId);

    const iceCandidateParsed: RTCIceCandidateInit = JSON.parse(iceCandidate);
    await userConnection.rtcConnection.addIceCandidate(new RTCIceCandidate(iceCandidateParsed));
  }

  // Метод находит, а если не нашел - то создает экземпляр подключения к пользователю
  private async findOrCreateUserConnectionByUserId(userId: string): Promise<UserConnection> {
    let userConnection = this.connections.find(c => c.otherUserHubConnectionId === userId);

    if (!userConnection) {
      let rtcConnectionWithUser = new RTCPeerConnection(this.rtcConfiguration);

      userConnection = new UserConnection(userId, rtcConnectionWithUser);
      this.connections.push(userConnection);
      this.connectionsBehaviorSubject.next(this.connections);

      console.log("Создано подключение с пользователем: " + userId);
    } else {
      console.log("Найдено подключение с пользователем: " + userId);
    }

    return userConnection;
  }

  // Регистрация хендлера обмена ICE-кандидатами
  private registerIceCandidatesEventHandler(rtcConnectionWithUser: RTCPeerConnection, sendTo: string): void {
    rtcConnectionWithUser.onicecandidate = async event => {
      if (event.candidate) {
        console.log('WebRTC: создан новый ICE candidate');
        await this.currentUserHubConnection!
          .invoke("SendIceCandidateToOtherPeer", sendTo, JSON.stringify(event.candidate.toJSON()));
      } else {
        console.log('WebRTC: сбор ICE кандидатов завершен');
      }
    };
  }

  // Метод отключения пользователя от созвона
  private async disconnectUser(userId: string) {
    console.log("Ищем пользователя с Id " + userId + ", чтоб отключить");

    let index = this.connections.findIndex(c => c.otherUserHubConnectionId === userId);
    if (index > -1) {
      console.log("Отключаем пользователя " + userId);
      this.connections.splice(index, 1);
    } else {
      console.log("Пользователь не найден");
    }
    this.connectionsBehaviorSubject.next(this.connections);
  }
}
