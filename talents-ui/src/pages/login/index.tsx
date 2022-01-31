import React, { Component } from "react";
import { Form, Input, Button, message, Alert } from "antd";
import { LockOutlined, MailOutlined } from "@ant-design/icons";
import "./style.css";
import { Link } from "react-router-dom";
import { ILoginModel } from "../../models/account/ILoginModel";
import { IAlertModel } from "../../models/IAlertModel";
import AccountService from "../../services/account-service";
import { LOCALSTORAGE } from "../../models/constants";

interface IState {
    isLoading: boolean,
    alert: IAlertModel
}
export default class Login extends Component<{}, IState> {
    state = {
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        }
    }

    _onFinish = async (values: any) => {
        const data: ILoginModel = {
            email: values.email,
            password: values.password
        }
        this._toggleLoading(true);
        try {
            const result = await AccountService.login(data);
            if (result.isSuccess) {
                message.success("Login successful!");
                this.setState({ alert: { show: true, message: `Login successful.`, isSuccess: true } });
                if (result.message === "should-set-password") {
                    window.location.href = `/resetpassword/${result.data}`;
                }
                else {
                    localStorage.setItem(LOCALSTORAGE.TOKEN, result.data);
                    window.location.href = '/dashboard';
                }
            }
            else {
                message.error(result.message);
                this.setState({ alert: { show: true, message: result.message, isSuccess: false } });
            }
        }
        catch (error) {
            message.error("An error occured while processing request");
        }
        this._toggleLoading(false);
    }

    _toggleLoading = (isLoading: boolean) => this.setState({ isLoading });
    render() {
        return (
            <>
                <fieldset disabled={this.state.isLoading}>
                    <div style={{ marginTop: '10vh' }}>
                        <h1 className="text-center">Sign in</h1>
                        <Form
                            name="normal_login"
                            className="login-form"
                            onFinish={this._onFinish}>
                            {this.state.alert.show && <Alert type={this.state.alert.isSuccess ? 'success' : 'error'} message={this.state.alert.message} />}
                            <p></p>
                            <Form.Item
                                name="email"
                                rules={[
                                    {
                                        required: true,
                                        message: 'Please input your email address!',
                                    },
                                ]}>
                                <Input type="email" prefix={<MailOutlined className="site-form-item-icon" />} placeholder="Email" />
                            </Form.Item>
                            <Form.Item
                                name="password"
                                rules={[
                                    {
                                        required: true,
                                        message: 'Please input your Password!',
                                    },
                                ]}>
                                <Input.Password
                                    prefix={<LockOutlined className="site-form-item-icon" />}
                                    placeholder="Password"
                                />
                            </Form.Item>
                            <Form.Item>
                                <Link to="/forgotpassword" className="login-form-forgot">Forgot password</Link>
                            </Form.Item>

                            <Form.Item>
                                <Button loading={this.state.isLoading} type="primary" htmlType="submit" className="login-form-button">Log in</Button>
                            </Form.Item>

                            <div className="text-center">
                                <Link to="/register">Register a new account</Link>
                            </div>
                        </Form>
                    </div>
                </fieldset>
            </>
        );
    }
}