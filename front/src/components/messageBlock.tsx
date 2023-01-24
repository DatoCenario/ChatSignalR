import React, { useState } from "react";
import { Modal } from "antd";
import styled from "styled-components";

export const MessageBlock: React.FC<{
  id: number;
  text: string;
  sender: string;
  time: string;
  isMe: boolean;
  images: string[];
}> = ({ text, sender, time, isMe, id, images }) => {
  const [modalVisible, setModalVisible] = useState(false);
  const [currentImage, setCurrentImage] = useState("");

  const handleImageClick = (image: string) => {
    setCurrentImage(image);
    setModalVisible(true);
  };

  return (
    <MessageBlockWrapper
      key={id}
      isMe={isMe}
      text={text}
      sender={sender}
      time={time}
    >
      <div
        style={{
          display: "block",
        }}
      >
        {images.map((i, index) => (
          <img
            src={i}
            key={index}
            onClick={() => handleImageClick(i)}
            style={{
              border: "1px solid #ccc",
              width: "100px",
              height: "100px",
              objectFit: "cover",
            }}
          />
        ))}
        {text && (
          <div key={id} className="message-item">
            {!isMe && <Sender>{sender}</Sender>}
            <Text>{text}</Text>
            <Time>{time}</Time>
          </div>
        )}
        {!text && time && <Time color="white">{time}</Time>}
      </div>
      <Modal
        visible={modalVisible}
        onCancel={() => setModalVisible(false)}
        footer={null}
      >
        <img src={currentImage} style={{ width: "100%" }} />
      </Modal>
    </MessageBlockWrapper>
  );
};
MessageBlock.defaultProps = {
  text: "",
  sender: "",
  time: "",
  isMe: false,
};

const MessageBlockWrapper = styled.div<{
  text: string;
  sender: string;
  time: string;
  isMe: boolean;
}>`
  .message-item {
    align-self: flex-start;
    overflow-wrap: break-word;
    word-break: break-all;
    padding: 10px;
    border-radius: 20px;
    background-color: ${(props) => (props.isMe ? "#e6f7ff" : "#f5f5d7")};
    word-wrap: break-word;
    transition: opacity 1s;
  }

  .message-item:hover {
    opacity: 50%;
  }
`;

const Sender = styled.div`
  font-weight: bold;
  margin-bottom: 5px;
`;
const Time = styled.div<{ color?: string }>`
  font-size: 12px;
  color: ${(props) => props.color || "#888"};
`;
const MessageWrapper = styled.div`
  display: flex;
  flex-direction: column;
`;

const Text = styled.div``;
