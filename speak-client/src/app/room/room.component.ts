import { Component, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";
import { HubConnectionService } from "../hub-connection.service";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {
  get userJoinedRoom(){
    return this.webRtcService.userJoinedRoom;
  }

  constructor(private hubConnectionService: HubConnectionService, private webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
    await this.hubConnectionService.connectToHub();
  }
}
