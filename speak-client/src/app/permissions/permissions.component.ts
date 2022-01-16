import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-permissions',
  templateUrl: './permissions.component.html',
  styleUrls: ['./permissions.component.css']
})
export class PermissionsComponent implements OnInit {

  localStream?: MediaStream;

  constructor() { }

  async ngOnInit(): Promise<void> {
    this.localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
  }

  async grantAccess() {
    this.localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
  }

}
