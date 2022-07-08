import { Component, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";
import {HubConnectionService} from "../hub-connection.service";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {

  constructor(private hubConnectionService: HubConnectionService, public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
    await this.hubConnectionService.connectToHub();
  }
}
