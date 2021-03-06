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

  constructor(public route: ActivatedRoute, public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {


    this.webRtcService.connectionsObservable
      .subscribe(connections => this.otherUserConnections = connections);
  }

  getCameraWidth(sceneWidth: number, sceneHeight: number): number {
    let windowArea = sceneWidth * sceneHeight;
    let maxCameraArea = windowArea / (this.otherUserConnections.length + 1);

    return (Math.sqrt(maxCameraArea / 144) * 16) * 0.6;
  }

  getCameraHeight(sceneWidth: number, sceneHeight: number): number {
    let windowArea = sceneWidth * sceneHeight;
    let maxCameraArea = windowArea / (this.otherUserConnections.length + 1);

    return (Math.sqrt(maxCameraArea / 144) * 9) * 0.6;
  }
}
