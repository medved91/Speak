import {Component, OnInit} from '@angular/core';
import {WebRtcService} from "../web-rtc.service";
import {UserConnection} from "../user-connection";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-call',
  templateUrl: './call.component.html',
  styleUrls: ['./call.component.css']
})
export class CallComponent implements OnInit {

  currentRoomId!: string;

  otherUserConnections: UserConnection[] = [];

  constructor(public route: ActivatedRoute, public webRtcService: WebRtcService) {  }

  async ngOnInit(): Promise<void> {
    let id = this.route.snapshot.paramMap.get('id');
    this.currentRoomId = id!;

    this.webRtcService.localStream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});

    this.webRtcService.connectionsObservable
      .subscribe(connections => this.otherUserConnections = connections);

    await this.webRtcService.startConnection();
    await this.webRtcService.joinRoom(this.currentRoomId);
  }
}
