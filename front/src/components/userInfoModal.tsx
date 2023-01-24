import { Avatar, Modal } from "antd";
import React from "react";
import styled from "styled-components";
import { ChatUser } from "./chatList";

export interface UserInfoModalProps {
  user: ChatUser;
  onClose?: () => void;
}

export const UserInfoModal: React.FC<UserInfoModalProps> = (
  props: UserInfoModalProps
) => {
  return (
    <Modal
      okButtonProps={{ style: { display: "none" } }}
      cancelButtonProps={{ style: { display: "none" } }}
      onCancel={() => {
        props.onClose && props.onClose();
      }}
      open={true}
    >
      <UserInfoWrapper>
        <h1 className="user-header">{`${props.user.firstName} ${props.user.lastName}`}</h1>
        <Avatar className="user-avatar" />
      </UserInfoWrapper>
    </Modal>
  );
};

const UserInfoWrapper = styled.div`
  display: flex;
  flex-direction: column;

  .user-avatar {
    width: 100px;
    height: 100px;
    align-self: center;
    margin-top: 7%;
  }

  .user-header {
    align-self: center;
  }
`;
