import { Injectable } from '@angular/core';
import { UserConnection } from "./user-connection";
import { BehaviorSubject } from "rxjs";
import { HubConnectionService } from "./hub-connection.service";
import { ChatService } from "./chat.service";

@Injectable({ providedIn: 'root' })
export class WebRtcService {
  get userJoinedRoom(): boolean {
    return this._userJoinedRoom;
  }

  private set userJoinedRoom(value: boolean) {
    this._userJoinedRoom = value;
  }

  rtcConfiguration: RTCConfiguration = {
    iceServers: [
      {
        urls: ['stun:stun1.l.google.com:19302', 'stun:stun2.l.google.com:19302']
      },
      {
        urls: "stun:openrelay.metered.ca:80",
      },
      {
        urls: "turn:openrelay.metered.ca:80",
        username: "openrelayproject",
        credential: "openrelayproject",
      },
      {
        urls: "turn:openrelay.metered.ca:443",
        username: "openrelayproject",
        credential: "openrelayproject",
      },
      {
        urls: "turn:openrelay.metered.ca:443?transport=tcp",
        username: "openrelayproject",
        credential: "openrelayproject",
      }
    ],
    iceCandidatePoolSize: 10
  };

  localStream?: MediaStream;
  localUserName?: string;

  private _userJoinedRoom = false;

  private connections: UserConnection[] = [];
  private connectionsBehaviorSubject = new BehaviorSubject<UserConnection[]>([]);
  public connectionsObservable = this.connectionsBehaviorSubject.asObservable();

  constructor(private hubConnectionService: HubConnectionService, private chatService: ChatService) {
    console.log("Инициализация WebRtcService");

    // Бэк вызывает этот метод у всех других клиентов, когда пользователь оповещает всех о своем подключении с помощью метода "NotifyOthersAboutNewUser"
    this.hubConnectionService.currentUserHubConnection.on("SendOfferToUser",
      async (toUserConnectionId: string) => this.sendOfferToUser(toUserConnectionId));

    // Бэкенд вызовет этот метод при отправке текущему юзеру оффера на подключение
    this.hubConnectionService.currentUserHubConnection.on("ReceiveOffer",
      async (offer: string, fromUserConnectionId: string) => await this.receiveOfferFromUser(fromUserConnectionId, offer));

    // Этот метод будет вызван с бэка при отправке текущему юзеру ответа на оффер
    this.hubConnectionService.currentUserHubConnection.on("ReceiveAnswer",
      async (answer: string, fromUserId: string) => await this.receiveAnswerFromUser(fromUserId, answer));

    // Метод будет вызван с бэка при отправке текущему юзеру ICE-кандидатов
    this.hubConnectionService.currentUserHubConnection.on("ReceiveIceCandidate",
      async (iceCandidate: string, fromUserId: string) => await this.receiveIceCandidateFromUser(fromUserId, iceCandidate));

    // Этот метод вызывается с бэка, когда пользователь отключается
    this.hubConnectionService.currentUserHubConnection.on("DisconnectUser",
      async (userId: string) => await this.disconnectUser(userId));
  }

  /// Метод подключения пользователя к комнате. Уведомляет остальных пользователей в комнате, провоцируя обмен офферами
  async joinRoom(roomId: string): Promise<void> {
    this.userJoinedRoom = true;

    await this.hubConnectionService.currentUserHubConnection.invoke("SetMyName", this.localUserName);
    await this.hubConnectionService.currentUserHubConnection.invoke("AddCurrentUserToRoom", roomId);

    await this.chatService.loadCurrentRoomChatMessages(roomId);

    let otherConnectedUserIds = await this.hubConnectionService.currentUserHubConnection
      .invoke<string[]>("GetOtherConnectedUsersInRoom", roomId);

    if(otherConnectedUserIds.length === 0) return;

    for (let userId of otherConnectedUserIds) await this.findOrCreateUserConnectionByUserId(userId);

    await this.hubConnectionService.currentUserHubConnection.invoke("NotifyOthersInRoomAboutNewUser", roomId);
  }

  // Метод отправки оффера другому пользователю
  private async sendOfferToUser(toUserId: string): Promise<void> {
    let connectionWithUser = await this.findOrCreateUserConnectionByUserId(toUserId);

    this.localStream!.getTracks().forEach(track => connectionWithUser.rtcConnection.addTrack(track, this.localStream!));

    let offer = await connectionWithUser.rtcConnection.createOffer();
    await connectionWithUser.rtcConnection.setLocalDescription(offer);

    await this.registerIceCandidatesEventHandler(connectionWithUser.rtcConnection, toUserId);

    await this.hubConnectionService.currentUserHubConnection.invoke("SendOffer", toUserId, JSON.stringify(offer));
  }

  // Метод получения оффера от другого пользователя
  private async receiveOfferFromUser(fromUserConnectionId: string, offer: string) {
    console.log("Получен оффер от: " + fromUserConnectionId);

    let connectionWithUser = await this.findOrCreateUserConnectionByUserId(fromUserConnectionId);

    this.localStream!.getTracks().forEach(track => connectionWithUser.rtcConnection.addTrack(track, this.localStream!));

    const description: RTCSessionDescriptionInit = JSON.parse(offer);
    await connectionWithUser.rtcConnection.setRemoteDescription(description);

    const answer = await connectionWithUser.rtcConnection.createAnswer();
    await connectionWithUser.rtcConnection.setLocalDescription(answer);

    await this.registerIceCandidatesEventHandler(connectionWithUser.rtcConnection, fromUserConnectionId);

    await this.hubConnectionService.currentUserHubConnection
      .invoke("SendAnswerToUser", connectionWithUser.otherUser.connectionId, JSON.stringify(answer));
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
    let userConnection = this.connections.find(c => c.otherUser.connectionId === userId);

    if (!userConnection) {
      let rtcConnectionWithUser = new RTCPeerConnection(this.rtcConfiguration);

      let otherUserName = await this.hubConnectionService.currentUserHubConnection
        .invoke<string>("GetUserName", userId);

      userConnection = new UserConnection({ connectionId: userId, name: otherUserName }, rtcConnectionWithUser);
      this.connections.push(userConnection);
      this.connectionsBehaviorSubject.next(this.connections);
    }

    return userConnection;
  }

  // Регистрация хендлера обмена ICE-кандидатами
  private registerIceCandidatesEventHandler(rtcConnectionWithUser: RTCPeerConnection, sendTo: string): void {
    rtcConnectionWithUser.onicecandidate = async event => {
      if (event.candidate) {
        console.log('WebRTC: создан новый ICE candidate');
        await this.hubConnectionService.currentUserHubConnection
          .invoke("SendIceCandidateToOtherPeer", sendTo, JSON.stringify(event.candidate.toJSON()));
      } else {
        console.log('WebRTC: сбор ICE кандидатов завершен');
      }
    };
  }

  // Метод отключения пользователя от созвона
  private async disconnectUser(userId: string) {
    let index = this.connections.findIndex(c => c.otherUser.connectionId === userId);
    if (index > -1) {
      console.log("Отключаем пользователя " + userId);
      this.connections.splice(index, 1);
    }

    this.connectionsBehaviorSubject.next(this.connections);
  }
}
