import { Component, OnInit } from '@angular/core';
import {WebRtcService} from "../web-rtc.service";
import {ActivatedRoute} from "@angular/router";
import {ChatMessage} from "../chat-message";

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  private currentRoomId?: string;
  currentRoomMessages?: ChatMessage[];

  messageToSend?: string;

  constructor(public route: ActivatedRoute, public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
    let id = this.route.snapshot.paramMap.get('id');
    this.currentRoomId = id!;

    this.webRtcService.chatMessagesObservable
      .subscribe(chatMessages => this.currentRoomMessages = chatMessages);
  }

  async sendMessage() {
    if (this.messageToSend && this.currentRoomId)
      await this.webRtcService.sendMessage(this.messageToSend, this.currentRoomId);

    this.messageToSend = '';
  }
}
