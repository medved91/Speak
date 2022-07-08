import {Injectable, OnInit} from '@angular/core';
import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {environment} from "../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class HubConnectionService implements OnInit {

  get currentUserHubConnection(): HubConnection {
    return this._currentUserHubConnection;
  }

  get currentUserId(): string {
    return this._currentUserId;
  }

  private readonly _currentUserHubConnection: HubConnection;
  private _currentUserId!: string;

  constructor() {
    this._currentUserHubConnection = new HubConnectionBuilder()
      .withUrl(environment.hubUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Debug)
      .build();
  }

  async ngOnInit(): Promise<void> {
  }

  async connectToHub() {
    await this.currentUserHubConnection.start();
    console.log('Успешно подключились к Signalling-серверу');
    this._currentUserId = await this.currentUserHubConnection.invoke<string>("GetConnectionId");
  }
}
