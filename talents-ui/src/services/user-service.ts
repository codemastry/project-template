import axios from "axios";
import { getFormData } from "../helpers/form-data-helper";
import { LOCALSTORAGE } from "../models/constants";
import { IResultModel } from "../models/IResultModel";
import { ICreateUpdateUserModel } from "../models/user/ICreateUpdateUserModel";
import { ICreateUpdateUserRoleItem } from "../models/user/ICreateUpdateUserRoleItem";
import { IUserModel } from "../models/user/IUserModel";

export default class UserService {
    static create = async (data: ICreateUpdateUserModel): Promise<IResultModel> => {
        var formData = getFormData(data);
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/user`, formData,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    console.log(error);
                    reject(error);
                });
        });
    }

    static update = async (data: ICreateUpdateUserModel): Promise<IResultModel> => {
        var formData = getFormData(data);
        return new Promise((resolve, reject) => {
            axios.put(`${process.env.REACT_APP_API_ENDPOINT}/api/user`, formData,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static delete = async (id: number): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.delete(`${process.env.REACT_APP_API_ENDPOINT}/api/user/${id}`,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static getById = async (id: number): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.get(`${process.env.REACT_APP_API_ENDPOINT}/api/user/${id}`,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static getAll = async (search?: string): Promise<Array<IUserModel>> => {
        var url = `${process.env.REACT_APP_API_ENDPOINT}/api/user`;
        if (search !== '' && search !== undefined && search !== null)
            url += `?search=${search}`;

        return new Promise((resolve, reject) => {
            axios.get(url,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static toggleLock = async (id: number, isLocked: boolean): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/user/togglelock`, { id, isLocked },
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static roleItems = async (id: number): Promise<Array<ICreateUpdateUserRoleItem>> => {
        var url = `${process.env.REACT_APP_API_ENDPOINT}/api/user/roleitems/${id}`;

        return new Promise((resolve, reject) => {
            axios.get(url,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static impersonate = async (id: number): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/user/impersonate/${id}`, {},
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }

    static getSpecialPermissions = async (id: number): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.get(`${process.env.REACT_APP_API_ENDPOINT}/api/user/specialpermissions/${id}`,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }
    
    static saveSpecialPermissions = async (id: number, grantedPermissions: Array<string>): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/user/specialpermissions/${id}`, grantedPermissions,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }
    
    static getCurrentUserPermissions = async (): Promise<Array<string>> => {
        return new Promise((resolve, reject) => {
            axios.get(`${process.env.REACT_APP_API_ENDPOINT}/api/user/permissions`,
                {
                    headers: {
                        "Authorization": `bearer ${localStorage.getItem(LOCALSTORAGE.TOKEN)}`
                    }
                })
                .then(function (response) {
                    resolve(response.data);
                })
                .catch(function (error) {
                    reject(error);
                });
        });
    }
}