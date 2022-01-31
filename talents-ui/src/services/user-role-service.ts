import axios from "axios";
import { LOCALSTORAGE } from "../models/constants";
import { IResultModel } from "../models/IResultModel";
import { ICreateUpdateUserRoleModel } from "../models/user-role/ICreateUpdateUserRoleModel";
import { IUserRoleModel } from "../models/user-role/IUserRoleModel";

export default class UserRoleService {
    static create = async (data: ICreateUpdateUserRoleModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.post(`${process.env.REACT_APP_API_ENDPOINT}/api/userrole`, data,
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

    static update = async (data: ICreateUpdateUserRoleModel): Promise<IResultModel> => {
        return new Promise((resolve, reject) => {
            axios.put(`${process.env.REACT_APP_API_ENDPOINT}/api/userrole`, data,
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
            axios.delete(`${process.env.REACT_APP_API_ENDPOINT}/api/userrole/${id}`,
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

    static getById = async (id: number): Promise<IUserRoleModel> => {
        return new Promise((resolve, reject) => {
            axios.get(`${process.env.REACT_APP_API_ENDPOINT}/api/userrole/${id}`,
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

    static getAll = async (search?: string): Promise<Array<IUserRoleModel>> => {
        var url = `${process.env.REACT_APP_API_ENDPOINT}/api/userrole`;
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

    static getAllPermissionsForTreeView = async (): Promise<Array<any>> => {
        var url = `${process.env.REACT_APP_API_ENDPOINT}/api/userrole/allpermissionstree`;
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
}