import {Component, Input, OnInit} from '@angular/core';
import {WebRtcService} from "../web-rtc.service";

@Component({
  selector: 'app-top-bar',
  templateUrl: './top-bar.component.html',
  styleUrls: ['./top-bar.component.css']
})
export class TopBarComponent implements OnInit {

  @Input() serviceName?: string;

  constructor(public webRtcService: WebRtcService) { }

  ngOnInit(): void {

  }

  switchChat() {
    this.webRtcService.chatOpened = !this.webRtcService.chatOpened;
  }
}
