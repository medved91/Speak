import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { ChatMessage } from "../chat-message";
import { ChatService } from "../chat.service";

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  get chatOpened() {
    return this.chatService.chatOpened;
  }

  get currentUserId() {
    return this.chatService.currentUserId;
  }

  private currentRoomId?: string;
  currentRoomMessages?: ChatMessage[];

  messageToSend?: string;

  constructor(private route: ActivatedRoute, private chatService: ChatService) { }

  async ngOnInit(): Promise<void> {
    let id = this.route.snapshot.paramMap.get('id');
    this.currentRoomId = id!;

    this.chatService.chatMessagesObservable
      .subscribe(chatMessages => this.currentRoomMessages = chatMessages);
  }

  async sendMessage() {
    if (this.messageToSend && this.currentRoomId)
      await this.chatService.sendMessage(this.messageToSend, this.currentRoomId);

    this.messageToSend = '';
  }
}
