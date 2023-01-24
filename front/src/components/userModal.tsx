import { Button, Input, List, Modal } from "antd";
import axios from "axios";
import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { GetUsersRequest, GetUsersResponse, UserResponse } from "./models";

interface UserModalProps {
  onChoose?: (users: number[], chatName?: string) => void;
  onCancel?: () => void;
  onClose?: () => void;
}

export const UserModal: React.FC<UserModalProps> = (props: UserModalProps) => {
  const [chatName, setChatName] = useState<string>("");
  const [users, setUsers] = useState<UserResponse[]>([]);
  const [searchValue, setSearchValue] = useState("");
  const [selectedUsersIds, setSelectedUsersIds] = useState<number[]>([]);
  const [count, setCount] = useState(200);
  const [offset, setOffset] = useState(0);
  const [usersLeft, setUsersLeft] = useState(0);

  useEffect(() => {
    let req: GetUsersRequest = {
      additionalIds: selectedUsersIds,
      fullNameFilter: searchValue,
      count: count,
      offset: offset,
      excludeHasChatsWithMe: false,
      excludeMe: true,
    };

    axios
      .post<GetUsersResponse>("api/Chat/GetUsers", req)
      .then((response) => {
        setUsers(response.data.users);
        setUsersLeft(response.data.entitiesLeft);
      })
      .catch((error) => {
        console.log(error);
      });
  }, [searchValue, offset]);

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchValue(e.target.value);
  };

  const handleShowMore = () => {
    if (usersLeft > 0) {
      setOffset(offset + count);
    }
  };

  const handleSelect = (user: UserResponse) => {
    if (selectedUsersIds.includes(user.id)) {
      setSelectedUsersIds(selectedUsersIds.filter((u) => u != user.id));
    } else {
      setSelectedUsersIds([...selectedUsersIds, user.id]);
    }
  };

  const handleOk = () => {
    if (selectedUsersIds.length > 0 && props.onChoose) {
      props.onChoose(selectedUsersIds, chatName);
    }
  };

  const items = [
    <Input
      placeholder="User name"
      value={searchValue}
      onChange={handleSearch}
    />,
  ];

  if (selectedUsersIds.length > 1) {
    items.push(
      <Input
        required={true}
        value={chatName}
        onChange={(v) => setChatName(v.target.value)}
        placeholder="Chat name"
      />
    );
  }

  return (
    <Modal
      okButtonProps={{
        disabled: selectedUsersIds.length > 1 ? !chatName : false
      }}
      open={true}
      title="Choose User"
      onOk={handleOk}
      onCancel={props.onCancel}
      afterClose={props.onClose}
    >
      {items}
      <div>
        <UserListWrapper>
          <List
            itemLayout="horizontal"
            dataSource={users}
            renderItem={(item) => (
              <List.Item
                onClick={() => handleSelect(item)}
                className={`user-item ${
                  selectedUsersIds.includes(item.id) ? "selected" : ""
                }`}
              >
                <List.Item.Meta title={`${item.firstName} ${item.lastName}`} />
              </List.Item>
            )}
          />
          {/* <Button onClick={handleShowMore}>Show More</Button> */}
        </UserListWrapper>
      </div>
    </Modal>
  );
};

const UserListWrapper = styled.div`
  overflow-y: scroll;
  max-height: 50vh;

  .user-item {
    cursor: pointer;
  }

  .user-item:hover {
    background-color: #f0f2f5;
  }

  .user-item.selected {
    background-color: #e6f7ff;
  }
`;
