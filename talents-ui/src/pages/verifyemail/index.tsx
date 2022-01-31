import { Button, message, Result } from "antd";
import React, { Component } from "react";
import { RouteComponentProps } from "react-router";
import { Link } from "react-router-dom";
import { ITokenModel } from "../../models/account/ITokenModel";
import { IAlertModel } from "../../models/IAlertModel";
import AccountService from "../../services/account-service";

interface IState {
    isLoading: boolean,
    alert: IAlertModel,
    isVerifying: boolean,
    isVerificationSuccess: boolean,
    verificationMessage: string
}

export default class VerifyEmail extends Component<RouteComponentProps<{ token: string }>, IState> {
    state = {
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        },
        isVerifying: false,
        isVerificationSuccess: false,
        verificationMessage: ''
    }

    componentDidMount = async () => await this._process();

    _process = async () => {
        const data: ITokenModel = {
            email: '',
            token: this.props.match.params.token
        }
        this.setState({ isVerifying: true });
        try {
            const result = await AccountService.emailVerification(data);
            this.setState({ isVerificationSuccess: result.isSuccess, verificationMessage: result.message });
        }
        catch (error) {
            message.error("An error occured while processing request");
        }
        this.setState({ isVerifying: false });
    }


    render() {
        return (
            <>
                {this.state.isVerificationSuccess && !this.state.isVerifying && <Success />}
                {!this.state.isVerificationSuccess && !this.state.isVerifying && <Failed message={this.state.verificationMessage} />}
            </>
        )
    }
}

const Success = () => {
    return (
        <Result
            status="success"
            title="Successfully Verified Email Address!"
            subTitle="Your email address and registration was verified successfully. Please proceed to login."
            extra={[
                <Link to="/login" key="1">
                    <Button type="primary">
                        Click here to Login
                    </Button>
                </Link>
            ]}
        />
    )
}


const Failed = (props: any) => {
    return (
        <Result
            status="error"
            title="Email Verification Failed!"
            subTitle={props.message}
            extra={[
                <Link to="/resendverification" key="1">
                    <Button type="link" key="1">
                        Click here to resend email verification
                    </Button>
                </Link>
            ]}
        />
    )
}