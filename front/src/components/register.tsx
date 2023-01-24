import * as React from "react";
import { Form, Input, Checkbox, Button } from "antd";
import axios from "axios";
import { useRouter } from "next/router";
import styled from "styled-components";
interface Props {}

interface RegisterModel {
  firstName: string;
  lastName: string;
  mobileOrEmail: string;
  password: string;
  login: string;
  rememberMe: boolean;
}

interface LoginResponse {
  success: boolean;
  fieldErrors: string[];
}

export const RegisterPage: React.FC = () => {
  const router = useRouter();
  const [err, setErr] = React.useState<string[]>([]);

  const handleSubmit = async (values: FormData) => {
    event.preventDefault();

    try {
      const response = await axios.post<LoginResponse>(
        "/api/Login/Register",
        values
      );

      setErr(response.data.fieldErrors || []);

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
    <FormWrapper>
      <div className="form-container">
        <Form onFinish={handleSubmit} className="register-form">
          <Form.Item name="login">
            <Input name="login" placeholder="login" />
          </Form.Item>
          <Form.Item name="firstName">
            <Input name="firstName" placeholder="First name" />
          </Form.Item>
          <Form.Item name="lastName">
            <Input name="lastName" placeholder="Last name" />
          </Form.Item>
          <Form.Item name="mobileOrEmail">
            <Input name="mobileOrEmail" placeholder="Mobile or Email" />
          </Form.Item>
          <Form.Item name="password">
            <Input name="password" type="password" placeholder="Password" />
          </Form.Item>
          <Form.Item>
            <Checkbox name="rememberMe">Remember me</Checkbox>
          </Form.Item>
          <Form.Item className="center-items">
            <Button htmlType="submit" type="primary">
              Sign up
            </Button>
          </Form.Item>
        </Form>
      </div>
      {err.length > 0 && (
        <ErrorList>
          {err.map((e, index) => (
            <li key={index}>{e}</li>
          ))}
        </ErrorList>
      )}
    </FormWrapper>
  );
};

const FormWrapper = styled.div`
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

const ErrorList = styled.ul`
  list-style: none;
  padding: 0;
  margin: 0;
  color: red;
  font-size: 30px;
  text-align: center;
  li {
    margin-bottom: 10px;
  }
`;

