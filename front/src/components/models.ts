export interface SendMessageToChatEventArgs {
  chatId: number;
  messageId: number;
  senderName: string;
  text: string;
  sendTime: string;
  isMe: boolean;
  images: SendMessageToChatEventImage[];
}

export interface SendMessageToChatEventImage {
  base64String: string;
}

export interface CreateChatEventArgs {
  isPrivate: boolean;
  chatId: number;
  name: string;
  chatUsers: CreateChatEventUser[];
}

export interface CreateChatEventUser {
  id: number;
  firstName: string;
  lastName: string;
}

// Add graphQL to front and remove it !!
export interface GetChatsRequest {
  nameFilter: string;
  onlyUserChats: boolean;
}

export interface GetChatsResponse {
  isPrivate: boolean;
  name: string;
  id: number;
  lastMessage: string;
  lastMessageSender: string;
  lastMessageSendTime: string;
  chatUsers: ChatUserResponse[];
}

export interface ChatUserResponse {
  userId: number;
  firstName: string;
  lastName: string;
}

export interface GetChatMessagesRequest {
  offset: number;
  count: number;
  chatId: number;
}

export interface ChatImageResponse {
  base64Text: string;
}

export interface GetChatMessagesResponse {
  entitiesLeft: number;
  chatMessages: ChatMessageReponse[];
}

export interface ChatMessageReponse {
  id: number;
  sendTime: string;
  senderName: string;
  isMe: boolean;
  text: string;
  messageImages: ChatImageResponse[];
}

export interface GetUsersRequest {
  additionalIds: number[];
  excludeMe: boolean;
  excludeHasChatsWithMe: boolean;
  fullNameFilter?: string;
  phoneOrMailFilter?: string;
  idFilter?: number[];
  chatIdFilter?: number[];
  offset: number;
  count: number;
}

export interface GetUsersResponse {
  entitiesLeft: number;
  users: UserResponse[];
}

export interface UserResponse {
  id: number;
  firstName: string;
  lastName: string;
}

export interface LeaveChatEventArgs {
  chatId: number;
  leavedUserName: string;
}
