import { Component, OnInit } from '@angular/core';
import { WebRtcService } from "../web-rtc.service";

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
      this.stopAudio();
    else
      this.startAudio();

    this.micEnabled = !this.micEnabled;
  }

  switchCam() {
    if(this.camEnabled)
      this.stopVideo();
    else
      this.startVideo();

    this.camEnabled = !this.camEnabled;
  }


  private stopVideo(){
    let videoTracks = this.webRtcService.localStream?.getVideoTracks();
    if(videoTracks) videoTracks.forEach(track => track.enabled = false);
  }

  private stopAudio(){
    let audioTracks = this.webRtcService.localStream?.getAudioTracks();
    if(audioTracks) audioTracks.forEach(track => track.enabled = false);
  }

  private startVideo(){
    let videoTracks = this.webRtcService.localStream?.getVideoTracks();
    if(videoTracks) videoTracks.forEach(track => track.enabled = true);
  }

  private startAudio(){
    let audioTracks = this.webRtcService.localStream?.getAudioTracks();
    if(audioTracks) audioTracks.forEach(track => track.enabled = true);
  }
}
