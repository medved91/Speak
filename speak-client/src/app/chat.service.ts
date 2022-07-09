import { Injectable } from '@angular/core';
import { HubConnectionService } from "./hub-connection.service";
import { ChatMessage } from "./chat-message";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  get currentUserId(){
    return this.hubConnectionService.currentUserId;
  }

  chatOpened = false;

  private chatMessages?: ChatMessage[];
  private chatMessagesBehaviorSubject = new BehaviorSubject<ChatMessage[]>([]);
  public chatMessagesObservable = this.chatMessagesBehaviorSubject.asObservable();

  constructor(private hubConnectionService: HubConnectionService) {
    // Метод будет вызван с бэка при отправке сообщения в чат комнаты
    this.hubConnectionService.currentUserHubConnection.on("ReceiveChatMessages",
      async (roomId: string, chatMessages: ChatMessage[]) => await this.receiveChatMessages(roomId, chatMessages));
  }

  async loadCurrentRoomChatMessages(roomId: string) {
    await this.hubConnectionService.currentUserHubConnection.invoke("GetChatMessages", roomId);
  }

  async sendMessage(messageText: string, roomId: string) {
    await this.hubConnectionService.currentUserHubConnection.invoke("SendChatMessage", messageText, roomId);
  }

  async receiveChatMessages(roomId: string, roomMessages: ChatMessage[]) {
    this.chatMessages = roomMessages;
    this.chatMessagesBehaviorSubject.next(this.chatMessages);
  }
}
