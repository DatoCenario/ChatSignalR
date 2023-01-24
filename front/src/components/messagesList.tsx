import React, { useEffect, useRef, useState } from "react";
import { List, Input, Button } from "antd";
import styled from "styled-components";
import { MessageBlock } from "./messageBlock";

export interface MessagesListProps {
  messages: Message[];
}

export interface Message {
  id: number;
  text?: string;
  sender: string;
  time: string;
  isMe: boolean;
  imagesHrefs?: string[];
  images: string[];
}

export const MessagesList: React.FC<MessagesListProps> = (
  props: MessagesListProps
) => {
  const listRef = useRef<HTMLUListElement | null>(null);
  const lastMessageRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (lastMessageRef.current) {
      lastMessageRef.current.scrollIntoView();
    }
  }, [props.messages]);

  return (
    <MessagesListWrapper ref={listRef}>
      {props.messages.map((item) => (
        <div className={"message-row" + (item.isMe ? "-me" : "")}>
          <div ref={lastMessageRef} className={"message" + (item.isMe ? "-me" : "")}>
            <MessageBlock
              key={item.id}
              id={item.id}
              isMe={item.isMe}
              text={item.text}
              sender={item.sender}
              time={item.time}
              images={item.images}
            />
          </div>
        </div>
      ))}
    </MessagesListWrapper>
  );
};

const MessagesListWrapper = styled.div`
  display: flex;
  flex-direction: column;
  overflow-x: hidden;
  width: 100%;

  .message-row {
    display: flex;
    flex-direction: row;
    justify-content: left;
  }

  .message-row-me {
    display: flex;
    flex-direction: row;
    justify-content: right;
  }

  .message {
    display: flex;
    padding-left: 5%;
    position: relative;
    padding-bottom: 10vh;
    max-width: 40%;
  }

  .message-me {
    display: flex;
    padding-right: 5%;
    position: relative;
    padding-bottom: 10vh;
    max-width: 40%;
    align-self: flex-end;
  }
`;

export default MessagesList;
