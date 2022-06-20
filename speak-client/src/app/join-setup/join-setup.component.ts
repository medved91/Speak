import { Component, OnInit } from '@angular/core';
import {WebRtcService} from "../web-rtc.service";

@Component({
  selector: 'app-join-setup',
  templateUrl: './join-setup.component.html',
  styleUrls: ['./join-setup.component.css']
})
export class JoinSetupComponent implements OnInit {

  constructor(public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
    this.webRtcService.localStream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});
  }

  joinCall() {
    this.webRtcService.userCompletedJoinSetup = true;
  }
}
