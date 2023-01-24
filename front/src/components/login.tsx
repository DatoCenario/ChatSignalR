import * as React from "react";
import { Form, Input, Checkbox, Button } from "antd";
import axios from "axios";
import { useRouter } from "next/router";
import styled from "styled-components";
interface Props {}

interface RegisterModel {
  password: string;
  login: string;
  rememberMe: boolean;
}

interface LoginResponse {
  success: boolean;
}

export const LoginPage: React.FC = () => {
  const router = useRouter();

  const handleSubmit = async (values: FormData) => {
    event.preventDefault();

    try {
      const response = await axios.post<LoginResponse>(
        "/api/Login/Login",
        values
      );

      if (response.data.success) {
        router.replace("messenger");
      }

      // Handle successful login
      // You might redirect the user to a protected page, for example
    } catch (error) {
      // Handle failed login
      // Update formError state, for example
    }
  };

  return (
    <ForWrapper>
      <div className="form-container">
        <Form onFinish={handleSubmit} className="register-form">
          <Form.Item name="login">
            <Input name="login" placeholder="login" />
          </Form.Item>
          <Form.Item name="password">
            <Input name="password" type="password" placeholder="Password" />
          </Form.Item>
          <Form.Item>
            <Checkbox name="rememberMe">Remember me</Checkbox>
          </Form.Item>
          <Form.Item className="center-items">
            <Button htmlType="submit" type="primary">Log in</Button>
          </Form.Item>
        </Form>
      </div>
    </ForWrapper>
  );
};

const ForWrapper = styled.div`
  .center-items {
    text-align: center;
  }

  .form-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
  }
  .register-form {
    width: 600px;
    background: #fff;
    padding: 40px;
    border-radius: 10px;
    box-shadow: 0px 0px 10px #ccc;
  }
`;
