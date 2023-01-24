import React, { useState } from "react";
import { List, Input, Avatar, Button } from "antd";
import styled from "styled-components";

export interface Chat {
  id: number;
  name: string;
  href?: string;
  lastMessageSender?: string;
  lastMessage?: string;
  lastMessageSendTime?: string;
  isPrivate: boolean;
  users: ChatUser[];
}

export interface ChatUser {
  id: number;
  firstName: string;
  lastName: string;
}

export interface ChatListProps {
  onChatChange?: (chatId: number) => void;
  onPlusButtonClick?: () => void;
  chats: Chat[];
}

export const ChatList: React.FC<ChatListProps> = (props: ChatListProps) => {
  const [selectedChatId, setSelectedChatId] = useState(1);
  const [searchValue, setSearchValue] = useState("");

  const handleSelectChat = (id: number) => {
    setSelectedChatId(id);
    props.onChatChange && props.onChatChange(id);
  };

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchValue(e.target.value);
  };

  const filteredChats = props.chats.filter((chat) =>
    chat.name?.toLowerCase().includes(searchValue.toLowerCase())
  );

  return (
    <ChatListWrapper>
      <div className="context-menu">
        <Input
          className="search-input"
          placeholder="Search for a chat"
          value={searchValue}
          onChange={handleSearch}
        />
        <Button
          className="plus-button"
          onClick={() => {
            props.onPlusButtonClick && props.onPlusButtonClick();
          }}
        >
          +
        </Button>
      </div>

      <List
        rowKey={(item) => item.id}
        itemLayout="horizontal"
        dataSource={filteredChats}
        renderItem={(item) => (
          <List.Item
            className={`chat-item ${
              item.id === selectedChatId ? "selected" : ""
            }`}
            onClick={() => handleSelectChat(item.id)}
          >
            <List.Item.Meta
              avatar={<Avatar src={item.href} />}
              title={item.name}
              description={`${item.lastMessage || ""} ${
                item.lastMessageSendTime || ""
              }`}
            />
          </List.Item>
        )}
      />
    </ChatListWrapper>
  );
};

const ChatListWrapper = styled.div`
  .search-input {
  }

  .plus-button {
    flex: 2;
  }

  .context-menu {
    display: flex;
    flex-dirrection: row;
  }

  .chat-item {
    cursor: pointer;
  }

  .chat-item:hover {
    background-color: #f0f2f5;
  }

  .chat-item.selected {
    background-color: #e6f7ff;
  }
`;

export default ChatList;
