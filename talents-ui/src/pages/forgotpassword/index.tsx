import React, { Component } from "react";
import { Form, Input, Button, message, Alert } from "antd";
import { MailOutlined } from "@ant-design/icons";
import "./style.css";
import { Link } from "react-router-dom";
import { IAlertModel } from "../../models/IAlertModel";
import AccountService from "../../services/account-service";
import { ITokenModel } from "../../models/account/ITokenModel";

interface IState {
    isLoading: boolean,
    alert: IAlertModel
}
export default class ForgotPassword extends Component<{}, IState> {
    state = {
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        }
    }

    _onFinish = async (values: any) => {
        const data: ITokenModel = {
            email: values.email,
            token: ''
        }
        this._toggleLoading(true);
        try {
            const result = await AccountService.requestResetPassword(data);
            if (result.isSuccess) {
                message.success("Success");
                this.setState({ alert: { show: true, message: `A reset password link was sent to ${data.email}.`, isSuccess: true } });
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
                        <h1 className="text-center">Forgot Password</h1>
                        <Form
                            className="forgotpassword-form"
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
                                <Button loading={this.state.isLoading} type="primary" htmlType="submit" className="forgotpassword-form-button">Submit</Button>
                            </Form.Item>

                            <div className="text-center">
                                <Link to="/register">Register a new account</Link>
                            </div>
                            <p></p>
                            <div className="text-center">
                                <Link to="/login">Login</Link>
                            </div>
                        </Form>
                    </div>
                </fieldset>
            </>
        );
    }
}