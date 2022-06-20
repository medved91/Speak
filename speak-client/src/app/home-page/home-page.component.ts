import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {

  roomId: string;
  private seed: string = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';


  constructor() {
    this.roomId = HomePageComponent.randomString(15, this.seed);
  }

  ngOnInit(): void {
  }

  private static randomString(length: number, chars: string) {
    let result = '';
    for (let i = length; i > 0; --i) result += chars[Math.floor(Math.random() * chars.length)];

    return result;
  }

}
