import axios from "axios";
import { LOCALSTORAGE } from "../models/constants";
import { ILoginModel } from "../models/account/ILoginModel";
import { IResultModel } from "../models/IResultModel";
import { IRegisterModel } from "../models/account/IRegisterModel";
import { ITokenModel } from "../models/account/ITokenModel";
import { IResetPasswordModel } from "../models/account/IResetPasswordModel";

export default class AccountService {
    static isLoggedIn = (): boolean => {
        const token = localStorage.getItem(LOCALSTORAGE.TOKEN);
        return !(token === null || token === '');
    }

    static checkAuth = async (): Promise<boolean> => {
        return new Promise((resolve, reject) => {
            axios.get(`${process.env.REACT_APP_API_ENDPOINT}/api/account/checkauth`,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    localStorage.clear();
                    window.location.href = "/login";
                    reject(false);
                });
        });
    }

    static login = async (data: ILoginModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/account/login`, data)
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static register = async (data: IRegisterModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/account/register`, data)
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static emailVerification = async (data: ITokenModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/account/emailverification`, data)
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static resendEmailVerification = async (data: ITokenModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/account/resendemailverification`, data)
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static requestResetPassword = async (data: ITokenModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/account/requestresetpassword`, data)
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static resetPassword = async (data: IResetPasswordModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/account/resetpassword`, data)
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }
}