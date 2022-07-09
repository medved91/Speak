import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { ChatMessage } from "../chat-message";
import { HubConnectionService } from "../hub-connection.service";
import { ChatService } from "../chat.service";

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  private currentRoomId?: string;
  currentRoomMessages?: ChatMessage[];

  messageToSend?: string;

  constructor(public route: ActivatedRoute, public hubConnectionService: HubConnectionService, public chatService: ChatService) { }

  async ngOnInit(): Promise<void> {
    let id = this.route.snapshot.paramMap.get('id');
    this.currentRoomId = id!;

    this.chatService.chatMessagesObservable
      .subscribe(chatMessages => this.currentRoomMessages = chatMessages);

    await this.chatService.loadCurrentRoomChatMessages(this.currentRoomId);
  }

  async sendMessage() {
    if (this.messageToSend && this.currentRoomId)
      await this.chatService.sendMessage(this.messageToSend, this.currentRoomId);

    this.messageToSend = '';
  }
}
