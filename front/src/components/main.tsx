import React, { useEffect, useState } from "react";
import { Button, Dropdown, Input, message, Upload, UploadFile } from "antd";
import { PaperClipOutlined, UploadOutlined } from "@ant-design/icons";
import styled from "styled-components";
import { ChatList, Chat, ChatUser } from "./chatList";
import MessagesList, { Message } from "./messagesList";
import { ChatInfo } from "./chatInfo";
import * as signalR from "@microsoft/signalr";
import { HubConnection } from "@microsoft/signalr";
import {
  ChatMessageReponse,
  ChatUserResponse,
  CreateChatEventArgs,
  CreateChatEventUser,
  GetChatMessagesRequest,
  GetChatMessagesResponse,
  GetChatsRequest,
  GetChatsResponse,
  LeaveChatEventArgs,
  SendMessageToChatEventArgs,
  UserResponse,
} from "./models";
import axios from "axios";
import { UserModal } from "./userModal";
import { text } from "express";
import { ImageList, ImageListImage } from "./uploadedImagesList";
import { ImageEditModal } from "./imageEditModal";
import { base64ToUrl } from "./functions";
import { UserInfoModal } from "./userInfoModal";
import { Resizable, ResizableBox } from "react-resizable";

export const Messenger: React.FC = () => {
  const [selectedUser, setSelectedUser] = useState<ChatUser>(null);
  const [uploadedImages, setUploadedImages] = useState<ImageListImage[]>([]);
  const [selectedImage, setSelectedImage] = useState<ImageListImage>(null);
  const [messageText, setMessageText] = useState("");
  const [queriedChats, setQueriedChats] = useState<boolean>(false);
  const [userChooseVisible, setUserChooseVisible] = useState<boolean>(false);
  const [selectedChat, setSelectedChat] = useState<Chat>(null);
  const [chats, setChats] = useState<Chat[]>([]);
  const [messages, setMessages] = useState<Message[]>([]);
  const [connection, setConnection] = useState<null | HubConnection>(null);
  const [updater, setUpdater] = useState<boolean>(false);
  const [cachedMessages, setCachedMessages] = useState<{
    [chatId: number]: Message[];
  }>({});

  const [chatListWith, setChatListWidth] = useState<number>(100);

  const sendMessage = async (text: string, imagesBase64?: string[]) => {
    setMessageText("");
    setUploadedImages([]);
    await connection?.invoke("sendMessageToChat", {
      chatId: selectedChat.id,
      text: text,
      imagesBase64: imagesBase64,
    });
  };

  const queryChats = async () => {
    let chatReq: GetChatsRequest = {
      nameFilter: null,
      onlyUserChats: true,
    };

    type mapChatFc = (chat: GetChatsResponse) => Chat;
    type mapUserFc = (user: ChatUserResponse) => ChatUser;

    const mapChats: mapChatFc = (chat) => {
      return {
        id: chat.id,
        name: chat.name,
        lastMessage: chat.lastMessage,
        lastMessageSender: chat.lastMessageSender,
        lastMessageSendTime: chat.lastMessageSendTime,
        isPrivate: chat.isPrivate,
        users: chat.chatUsers.map(mapUser),
      };
    };

    const mapUser: mapUserFc = (user) => {
      return {
        id: user.userId,
        firstName: user.firstName,
        lastName: user.lastName,
      };
    };

    let chats = await axios.post<GetChatsResponse[]>(
      "/api/Chat/GetChats",
      chatReq
    );

    setChats(chats.data.map(mapChats));
  };

  const queryMessages = async (chatId: number | null) => {
    if (selectedChat == null && chatId == null) {
      return;
    }

    let messReq: GetChatMessagesRequest = {
      offset: 0,
      count: 0,
      chatId: chatId || selectedChat?.id,
    };

    let messages = cachedMessages[chatId || selectedChat?.id];
    if (!messages) {
      const mapMessage = (mess: ChatMessageReponse): Message => {
        return {
          id: mess.id,
          text: mess.text,
          sender: mess.senderName,
          isMe: mess.isMe,
          time: mess.sendTime,
          images: mess.messageImages.map((im) => base64ToUrl(im.base64Text)),
        };
      };

      let messagesData = await axios.post<GetChatMessagesResponse>(
        "/api/Chat/GetMessages",
        messReq
      );

      messages = messagesData.data.chatMessages.map(mapMessage);

      setCachedMessages({
        ...cachedMessages,
        [chatId || selectedChat?.id]: messages,
      });
    }

    setMessages(messages);
  };

  // Add graphQL to front and remove it !!

  if (!queriedChats) {
    queryChats();
    setQueriedChats(true);
  }

  const onSendMessage = async (args: SendMessageToChatEventArgs) => {
    const chat = chats.filter((c) => c.id === args.chatId).at(-1);
    const message: Message = {
      id: args.messageId,
      text: args.text,
      time: args.sendTime,
      sender: args.senderName,
      isMe: args.isMe,
      images: args.images.map((i) => base64ToUrl(i.base64String)),
    };

    if (!chat) {
      return;
    }

    if (chat.id === selectedChat?.id) {
      selectedChat.lastMessage = args.text;
      selectedChat.lastMessageSender = args.senderName;
      selectedChat.lastMessageSendTime = args.sendTime;
      setMessages([...messages, message]);
    }

    chat.lastMessage = args.text;
    chat.lastMessageSender = args.senderName;
    chat.lastMessageSendTime = args.sendTime;
    cachedMessages[chat.id]?.push(message);
    setCachedMessages(cachedMessages);
    setUpdater(!updater);
  };

  const onLeaveChat = (args: LeaveChatEventArgs) => {
    const chat = chats.filter((c) => c.id === args.chatId).at(-1);

    if (!chat) {
      return;
    }
  };

  const onCreateChat = (args: CreateChatEventArgs) => {
    const mapUser = (user: CreateChatEventUser): ChatUser => {
      return {
        id: user.id,
        firstName: user.firstName,
        lastName: user.lastName,
      };
    };

    setChats([
      ...chats,
      {
        id: args.chatId,
        name: args.name,
        isPrivate: args.isPrivate,
        users: args.chatUsers.map(mapUser),
      },
    ]);

    setCachedMessages({
      ...cachedMessages,
      [args.chatId]: [],
    });
  };

  useEffect(() => {
    const connect = new signalR.HubConnectionBuilder()
      .withUrl("/hub/messengerHub", {
        // skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    setConnection(connect);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start().catch((error) => console.log(error));
    }
  }, [connection]);

  connection?.off("chatCreateEvent");
  connection?.off("messageSendEvent");
  connection?.off("leaveChatEvent");
  connection?.on("leaveChatEvent", onLeaveChat);
  connection?.on("chatCreateEvent", onCreateChat);
  connection?.on("messageSendEvent", onSendMessage);

  const openUserWindow = () => {
    setUserChooseVisible(true);
  };

  const sendItems = [
    <Upload
      showUploadList={false}
      onChange={(info) => {
        if (info.file.status === "done") {
          // Get the file
          const file = info.file.originFileObj;
          // Create a FileReader
          const reader = new FileReader();
          // Read the file as a data URL
          reader.readAsDataURL(file);
          // When the file is read, update the state with the data URL
          reader.onloadend = () => {
            var canvas = document.createElement("canvas");

            // Create a new image element
            var img = new Image();

            // Set the source of the image to the file URL
            img.src = reader.result;

            // Draw the image on the canvas
            img.onload = function () {
              canvas.width = img.width;
              canvas.height = img.height;
              canvas.getContext("2d").drawImage(img, 0, 0);

              // Convert the canvas to a PNG data URL
              var pngDataURL = canvas.toDataURL("image/png");
              setUploadedImages([...uploadedImages, { url: pngDataURL }]);
            };
          };
        }
      }}
    >
      <Button disabled={!selectedChat} icon={<PaperClipOutlined />}></Button>
    </Upload>,

    <Input
      className="send-input"
      disabled={!selectedChat}
      value={messageText}
      onChange={(e) => setMessageText(e.target.value)}
      onKeyPress={async (e) => {
        if (e.key === "Enter") {
          await sendMessage(
            messageText,
            uploadedImages.map((i) =>
              i.url.replace(/^data:image\/(png|jpg);base64,/, "")
            )
          );
        }
      }}
      prefixCls="input-images-list"
      prefix={
        uploadedImages.length > 0 ? (
          <ImageList
            images={uploadedImages}
            onImageClick={(image) => {
              setSelectedImage(image);
            }}
            onClearClick={() => {
              setUploadedImages([]);
            }}
          />
        ) : null
      }
    />,

    <Button
      disabled={!selectedChat}
      type="primary"
      onClick={async (e) => {
        await sendMessage(
          messageText,
          uploadedImages.map((i) =>
            i.url.replace(/^data:image\/(png|jpg);base64,/, "")
          )
        );
      }}
    >
      Send
    </Button>,
  ];

  const chatItems = [
    <div className="chat-info">
      <ChatInfo
        onClick={() => {
          if (selectedChat?.isPrivate) {
            setSelectedUser(selectedChat.users.at(-1));
          }
        }}
        chatName={selectedChat?.name}
        privateChat={selectedChat?.isPrivate}
        onLeaveChat={() => {
          connection.invoke("leaveChat", selectedChat.id);
          setChats(chats.filter((c) => c.id !== selectedChat.id));
          setSelectedChat(null);
        }}
      />
    </div>,
  ];

  chatItems.push(
    <div className="messages-list">
      <MessagesList messages={messages} />
    </div>
  );

  chatItems.push(
    <div className="message-form">
      <div className="send-wrapper">{sendItems}</div>
    </div>
  );

  const items: JSX.Element[] = [
    <div key="item1">
      <div className="messenger-container">
        <Resizable
          height={100}
          width={chatListWith}
          onResize={(event, callbackData) => {
            setChatListWidth(event.clientX);
          }}
          handle={<span className="resize-handle"></span>}
        >
          <div
            className="chat-list-container"
            style={{
              width: `${chatListWith}px`,
              display: "flex",
            }}
          >
            <div className="chat-list">
              <ChatList
                chats={chats}
                onChatChange={(id: number) => {
                  let chat = chats.filter((c) => c.id === id).at(-1);
                  if (chat) {
                    setSelectedChat(chat);
                    queryMessages(chat.id);
                  }
                }}
                onPlusButtonClick={openUserWindow}
              />
            </div>
          </div>
        </Resizable>
        <div className="messages-list-container">{chatItems}</div>
      </div>
    </div>,
  ];

  if (selectedImage) {
    items.push(
      <ImageEditModal
        onClose={() => {
          setSelectedImage(null);
        }}
        onCancel={() => {
          setSelectedImage(null);
        }}
        image={selectedImage}
        onComplete={(newUrl) => {
          let newUploded = uploadedImages.filter(
            (i) => i.url != selectedImage.url
          );
          setUploadedImages([...newUploded, { url: newUrl }]);
          setSelectedImage(null);
        }}
      />
    );
  }

  if (selectedUser != null) {
    items.push(
      <UserInfoModal
        onClose={() => {
          setSelectedUser(null);
        }}
        user={selectedUser}
      />
    );
  }

  if (userChooseVisible === true) {
    items.push(
      <UserModal
        key="item2"
        onCancel={() => {
          setUserChooseVisible(false);
        }}
        onClose={() => {
          setUserChooseVisible(false);
        }}
        onChoose={(users, chatName) => {
          setUserChooseVisible(false);
          if (users.length === 1) {
            if (
              chats.filter(
                (c) =>
                  c.isPrivate &&
                  c.users.map((u) => u.id).filter((i) => users.includes(i))
                    .length > 0
              ).length > 0
            ) {
              return;
            }
            connection?.invoke("сreatePrivateChat", users[0]);
          } else {
            connection?.invoke("сreateChat", {
              usersIds: users,
              name: chatName,
            });
          }
        }}
      />
    );
  }

  return (
    <div>
      <MessengerWrapper>{items}</MessengerWrapper>
    </div>
  );
};

const MessengerWrapper = styled.div`
  display: flex;
  flex-direction: column;
  height: 95vh;

  .messenger-container {
    flex: 1;
    display: flex;
  }

  .message-form {
    bottom: 0;
    display: flex;
    justify-content: space-between;
  }

  .chat-info {
    flex: 0 5vh;
  }

  .chat-list {
    overflow-y: scroll;
    max-height: 100%;
    width: 100%;
    overflow-x: hidden;
  }

  .chat-list-container {
    display: flex;
    flex-direction: row;
    justify-content: right;
    max-height: 100vh;
    min-height: 100vh;
  }

  .messages-list-container {
    flex: 0 100%;
    display: flex;
    flex-direction: column;
  }

  .messages-list {
    flex: 0 60vh;
    background: linear-gradient(-45deg, #ee775e, #e73c7e, #23a6d5, #23d5ab);
    overflow-y: scroll;
    display: flex;
  }

  .send-input {
  }

  .send-wrapper {
    display: flex;
    flex-direction: row;
    max-width: 100%;
    min-width: 100%;
    width: 100%;
  }

  .uploaded-files-container {
    display: flex;
    flex-direction: row;
  }

  .input-images-list-prefix {
    max-width: 30%;
  }

  .resize-handle {
    align-self: flex-end;
    width: 3px;
    height: 100%;
    cursor: e-resize;
  }
`;

export default Messenger;
