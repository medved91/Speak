import { Component, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-join-setup',
  templateUrl: './join-setup.component.html',
  styleUrls: ['./join-setup.component.css']
})
export class JoinSetupComponent implements OnInit {
  private readonly currentRoomId: string;

  constructor(public route: ActivatedRoute, public webRtcService: WebRtcService) {
    let id = this.route.snapshot.paramMap.get('id');
    this.currentRoomId = id!;
  }

  async ngOnInit(): Promise<void> {
    this.webRtcService.localStream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});
  }

  async joinCall() {
    await this.webRtcService.joinRoom(this.currentRoomId);
  }
}
