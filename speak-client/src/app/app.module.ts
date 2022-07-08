import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatToolbarModule } from "@angular/material/toolbar";
import { TopBarComponent } from './top-bar/top-bar.component';
import { AppRoutingModule } from './app-routing.module';
import { MatCardModule } from "@angular/material/card";
import { MatButtonModule } from "@angular/material/button";
import { MatSidenavModule } from "@angular/material/sidenav";
import { RoomComponent } from './room/room.component';
import { MatGridListModule } from "@angular/material/grid-list";
import { MatInputModule } from "@angular/material/input";
import { MatIconModule } from "@angular/material/icon";
import { FormsModule } from "@angular/forms";
import { JoinSetupComponent } from './join-setup/join-setup.component';
import { CallComponent } from './call/call.component';
import { HomePageComponent } from './home-page/home-page.component';
import { SpecialCharactersDirective } from './special-characters.directive';
import { ControlsComponent } from './controls/controls.component';
import { ChatComponent } from './chat/chat.component';

@NgModule({
  declarations: [
    AppComponent,
    TopBarComponent,
    RoomComponent,
    JoinSetupComponent,
    CallComponent,
    HomePageComponent,
    SpecialCharactersDirective,
    ControlsComponent,
    ChatComponent
  ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        MatToolbarModule,
        AppRoutingModule,
        MatCardModule,
        MatButtonModule,
        MatSidenavModule,
        MatGridListModule,
        MatInputModule,
        MatIconModule,
        FormsModule
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
