import { MailOutlined } from "@ant-design/icons";
import "./style.css";
import { Alert, Button, Form, Input, message, Result } from "antd";
import React, { Component } from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { ITokenModel } from "../../models/account/ITokenModel";
import { IAlertModel } from "../../models/IAlertModel";
import AccountService from "../../services/account-service";

interface IState {
    isLoading: boolean,
    alert: IAlertModel,
}

export default class ResendVerification extends Component<RouteComponentProps<{ token: string }>, IState> {
    state = {
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        },
    }

    _onFinish = async (values: any) => {
        const data: ITokenModel = {
            email: values.email,
            token: ''
        }
        this._toggleLoading(true);
        try {
            const result = await AccountService.resendEmailVerification(data);
            if (result.isSuccess) {
                message.success(result.message);
                this.setState({ alert: { show: true, message: result.message, isSuccess: true } });
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
                        <h1 className="text-center">Resend Email Verification</h1>
                        <Form
                            className="resend-form"
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

                            <Form.Item>
                                <Button loading={this.state.isLoading} type="primary" htmlType="submit" className="resend-form-button">Send</Button>
                            </Form.Item>

                            <div className="text-center">
                                <Link to="/login">Login</Link>
                            </div>
                            <p></p>
                            <div className="text-center">
                                <Link to="/register">Register a new account</Link>
                            </div>
                        </Form>
                    </div>
                </fieldset>
            </>
        )
    }
}
