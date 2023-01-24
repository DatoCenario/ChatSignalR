import { Button, Dropdown, Menu } from "antd";
import styled from "styled-components";

export interface ChatInfoProps {
  chatName: string;
  privateChat?: boolean;
  onLeaveChat?: () => void;
  onMuteChat?: () => void;
  onSearch?: (value: string) => void;
  search?: string;
  onClick?: () => void;
}

export const ChatInfo: React.FC<ChatInfoProps> = (props: ChatInfoProps) => {
  const items = [
    <Menu.Item key="2" onClick={props.onMuteChat}>
      Mute Chat
    </Menu.Item>,
  ];

  if (!props.privateChat) {
    items.push(
      <Menu.Item key="1" onClick={props.onLeaveChat}>
        Leave Chat
      </Menu.Item>
    );
  }

  const menu = <Menu>{items}</Menu>;
  return (
    <ChatInfoWrapper onClick={() => {props.onClick && props.onClick()}}>
      <ChatName>{props.chatName}</ChatName>
      <Dropdown overlay={menu}>
        <Button>...</Button>
      </Dropdown>
    </ChatInfoWrapper>
  );
};

const ChatInfoWrapper = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  background-color: #f5f5f5;
  text-align: center;
`;

const ChatName = styled.h3`
  flex: 1;
`;
