import { Component, Input, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";
import { ChatService } from "../chat.service";

@Component({
  selector: 'app-top-bar',
  templateUrl: './top-bar.component.html',
  styleUrls: ['./top-bar.component.css']
})
export class TopBarComponent implements OnInit {
  get userJoinedRoom(){
    return this.webRtcService.userJoinedRoom;
  }

  @Input() serviceName?: string;

  constructor(private webRtcService: WebRtcService, private chatService: ChatService) { }

  ngOnInit(): void { }

  switchChat() {
    this.chatService.chatOpened = !this.chatService.chatOpened;
  }
}
