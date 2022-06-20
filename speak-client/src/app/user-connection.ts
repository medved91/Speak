import {User} from "../User";

export class UserConnection {
  otherUserMediaStream: MediaStream = new MediaStream();
  otherUser: User;
  rtcConnection: RTCPeerConnection;

  constructor(connectedToUser: User, rtcConnectionWithUser: RTCPeerConnection) {
    this.otherUser = connectedToUser;
    this.rtcConnection = rtcConnectionWithUser;

    this.rtcConnection.ontrack = event => {
      event.streams[0].getTracks().forEach(track => {
        console.log("Получен удаленный медиа-поток: " + track.kind);
        this.otherUserMediaStream.addTrack(track)
      })
    };
  }
}
