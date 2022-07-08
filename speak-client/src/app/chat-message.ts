import {User} from "./user";

export interface ChatMessage {
  fromUser: User;
  messageText: string;
  postedAt: Date;
}
