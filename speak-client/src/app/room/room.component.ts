import { Component, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {

  constructor(public webRtcService: WebRtcService) { }

  async ngOnInit(): Promise<void> {
  }
}
