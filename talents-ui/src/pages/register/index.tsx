import React, { Component } from "react";
import { Form, Input, Button, Select, message, Alert, Row, Col, Space, Result, FormInstance } from "antd";
import { IRegisterModel } from "../../models/account/IRegisterModel";
import AccountService from "../../services/account-service";
import { IAlertModel } from "../../models/IAlertModel";
import "./style.css";
import { Link } from "react-router-dom";

interface IState {
    isLoading: boolean,
    alert: IAlertModel
}
export default class Register extends Component<{}, IState> {
    formRef = React.createRef<FormInstance>();
    state = {
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        }
    }

    _onFinish = async (values: any) => {
        const data: IRegisterModel = {
            firstName: values.firstName,
            lastName: values.lastName,
            email: values.email,
            password: values.password,
            companyName: values.companyName,
            companySize: values.companySize
        }
        this._toggleLoading(true);
        try {
            const result = await AccountService.register(data);
            if (result.isSuccess) {
                this.formRef.current?.resetFields();
                message.success("Registration successful");
                this.setState({ alert: { show: true, message: `Registration successful. Please check your email for verification.`, isSuccess: true } });
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
                        <h1 className="text-center">Register</h1>
                        <Form
                            ref={this.formRef}
                            layout="vertical"
                            name="basic"
                            onFinish={this._onFinish}
                            className="register-form">
                            {this.state.alert.show && <Alert type={this.state.alert.isSuccess ? 'success' : 'error'} message={this.state.alert.message} />}
                            <Row gutter={16}>
                                <Col xs={24} sm={24} md={12}>
                                    <Form.Item
                                        label="First name"
                                        name="firstName"
                                        rules={[{ required: true, message: 'Please input your first name!' }]}>
                                        <Input />
                                    </Form.Item>
                                </Col>
                                <Col xs={24} sm={24} md={12}>
                                    <Form.Item
                                        label="Last name"
                                        name="lastName"
                                        rules={[{ required: true, message: 'Please input your last name!' }]}>
                                        <Input />
                                    </Form.Item>
                                </Col>
                                <Col xs={24} sm={24} md={12}>
                                    <Form.Item
                                        label="Company"
                                        name="companyName"
                                        rules={[{ required: true, message: 'Please input your company name!' }]}>
                                        <Input />
                                    </Form.Item>
                                </Col>
                                <Col xs={24} sm={24} md={12}>
                                    <Form.Item
                                        label="Company size"
                                        name="companySize"
                                        rules={[{ required: true, message: 'Please select company size!' }]}>
                                        <Select>
                                            <Select.Option value="1-500">1-500</Select.Option>
                                            <Select.Option value="501-1,000">501-1,000</Select.Option>
                                            <Select.Option value="1,001-5,000">1,001-5,000</Select.Option>
                                            <Select.Option value="5,001+">5,001+</Select.Option>
                                        </Select>
                                    </Form.Item>
                                </Col>
                                <Col xs={24}>
                                    <Form.Item
                                        label="Email address"
                                        name="email"
                                        rules={[
                                            { required: true, message: 'Please input your email address!' }
                                        ]}>
                                        <Input type="email" />
                                    </Form.Item>
                                </Col>
                                <Col xs={24}>
                                    <Form.Item
                                        label="Confirm email address"
                                        name="confirmEmail"
                                        rules={[
                                            { required: true, message: 'Please confirm your email address!' },
                                            ({ getFieldValue }) => ({
                                                validator(rule, value) {
                                                    if (!value || getFieldValue('email') === value) {
                                                        return Promise.resolve();
                                                    }
                                                    return Promise.reject('The email addresses do not match!');
                                                },
                                            })
                                        ]}>
                                        <Input type="email" />
                                    </Form.Item>
                                </Col>
                                <Col xs={24}>
                                    <Form.Item
                                        label="Password"
                                        name="password"
                                        rules={[
                                            { required: true, message: 'Please input your password!' }
                                        ]}>
                                        <Input.Password autoComplete="new-password" />
                                    </Form.Item>
                                </Col>
                                <Col xs={24}>
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
                                        <Input.Password autoComplete="new-password" />
                                    </Form.Item>
                                </Col>
                            </Row>
                            <Form.Item>
                                <Button loading={this.state.isLoading} type="primary" htmlType="submit">Get Started!</Button>
                            </Form.Item>
                            <div className="text-center">
                                <Link to="/login">Already have an account? Click here to login.</Link>
                            </div>
                        </Form>
                    </div>
                </fieldset>
            </>
        );
    }
}
