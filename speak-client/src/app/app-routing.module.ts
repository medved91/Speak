import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { RoomComponent } from "./room/room.component";
import {HomePageComponent} from "./home-page/home-page.component";

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: "full" },
  { path: 'room', redirectTo: '/home', pathMatch: "full" },
  { path: 'home', component: HomePageComponent },
  { path: 'room/:id', component: RoomComponent }
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
