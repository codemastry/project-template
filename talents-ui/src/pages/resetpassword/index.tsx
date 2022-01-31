import React, { Component } from "react";
import { Form, Input, Button, message, Alert } from "antd";
import { MailOutlined } from "@ant-design/icons";
import "./style.css";
import { Link, RouteComponentProps } from "react-router-dom";
import { IAlertModel } from "../../models/IAlertModel";
import AccountService from "../../services/account-service";
import { IResetPasswordModel } from "../../models/account/IResetPasswordModel";
import { LOCALSTORAGE } from "../../models/constants";

interface IState {
    isLoading: boolean,
    alert: IAlertModel
}
export default class ResetPassword extends Component<RouteComponentProps<{ token: string }>, IState> {
    state = {
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        }
    }

    _onFinish = async (values: any) => {
        const data: IResetPasswordModel = {
            password: values.password,
            token: this.props.match.params.token
        }
        this._toggleLoading(true);
        try {
            const result = await AccountService.resetPassword(data);
            if (result.isSuccess) {
                message.success("Success");
                this.setState({ alert: { show: true, message: `Login successful. Redirecting you to dashboard.`, isSuccess: true } });
                localStorage.setItem(LOCALSTORAGE.TOKEN, result.data);
                window.location.href = '/dashboard';
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
                        <h1 className="text-center">Reset Password</h1>
                        <Form
                            className="resetpassword-form"
                            onFinish={this._onFinish}
                            layout="vertical">
                            {this.state.alert.show && <Alert type={this.state.alert.isSuccess ? 'success' : 'error'} message={this.state.alert.message} />}
                            <p></p>
                            <Form.Item
                                label="New Password"
                                name="password"
                                rules={[
                                    { required: true, message: 'Please input your password!' }
                                ]}>
                                <Input.Password />
                            </Form.Item>
                            <Form.Item
                                label="Re type password"
                                name="reTypePassword"
                                rules={[
                                    { required: true, message: 'Please re-type your password!' },
                                    ({ getFieldValue }) => ({
                                        validator(rule, value) {
                                            if (!value || getFieldValue('password') === value) {
                                                return Promise.resolve();
                                            }
                                            return Promise.reject('The two passwords that you entered do not match!');
                                        },
                                    }),
                                ]}>
                                <Input.Password />
                            </Form.Item>
                            <Form.Item>
                                <Button loading={this.state.isLoading} type="primary" htmlType="submit" className="resetpassword-form-button">Submit</Button>
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