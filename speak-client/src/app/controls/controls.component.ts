import { Component, OnInit } from '@angular/core';
import {WebRtcService} from "../web-rtc.service";

@Component({
  selector: 'app-controls',
  templateUrl: './controls.component.html',
  styleUrls: ['./controls.component.css']
})
export class ControlsComponent implements OnInit {

  micEnabled: boolean = true;
  camEnabled: boolean = true;

  constructor(private webRtcService: WebRtcService) { }

  ngOnInit(): void {
  }

  switchMic() {
    if(this.micEnabled)
      this.webRtcService.stopAudio();
    else
      this.webRtcService.startAudio();

    this.micEnabled = !this.micEnabled;
  }

  switchCam() {
    if(this.camEnabled)
      this.webRtcService.stopVideo();
    else
      this.webRtcService.startVideo();

    this.camEnabled = !this.camEnabled;
  }
}
