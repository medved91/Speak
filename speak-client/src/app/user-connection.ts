export class UserConnection {
  otherUserMediaStream: MediaStream = new MediaStream();
  otherUserHubConnectionId: string;
  rtcConnection: RTCPeerConnection;

  constructor(connectedToUserId: string, rtcConnectionWithUser: RTCPeerConnection) {
    this.otherUserHubConnectionId = connectedToUserId;
    this.rtcConnection = rtcConnectionWithUser;

    this.rtcConnection.ontrack = event => {
      event.streams[0].getTracks().forEach(track => {
        console.log("Получен удаленный медиа-поток: " + track.kind);
        this.otherUserMediaStream.addTrack(track)
      })
    };
  }
}
