import { Component, OnInit } from '@angular/core';
import {WebRtcService} from "../web-rtc.service";
import {UserConnection} from "../user-connection";

@Component({
  selector: 'app-call',
  templateUrl: './call.component.html',
  styleUrls: ['./call.component.css']
})
export class CallComponent implements OnInit {

  otherUserConnections: UserConnection[] = [];

  constructor(public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
    this.webRtcService.localStream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});

    this.webRtcService.connectionsObservable
      .subscribe(connections => this.otherUserConnections = connections);

    await this.webRtcService.startConnection();
    await this.webRtcService.joinCall();
  }
}
