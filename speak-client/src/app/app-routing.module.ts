import { NgModule } from '@angular/core';
import { RouterModule, Routes } from "@angular/router";
import { PermissionsComponent } from "./permissions/permissions.component";
import { RoomComponent } from "./room/room.component";

const routes: Routes = [
  { path: '', redirectTo: 'permissions', pathMatch: "full" },
  { path: 'permissions', component: PermissionsComponent },
  { path: 'room', component: RoomComponent }
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
