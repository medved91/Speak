import { Component, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {

  otherUsersStreams: MediaStream[] = [];

  constructor(public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
    this.webRtcService.localStream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});

    this.webRtcService.connectionsObservable
      .subscribe(connections => this.otherUsersStreams = connections.map(connection => connection.otherUserMediaStream));

    await this.webRtcService.startConnection();
    await this.webRtcService.joinCall();
  }
}
