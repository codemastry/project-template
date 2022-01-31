import { ICreateUpdateUserRoleItem } from "./ICreateUpdateUserRoleItem";

export interface ICreateUpdateUserModel {
    id: number,
    firstName: string,
    lastName: string,
    email: string,
    password: string,
    isActive: boolean,
    isLocked: boolean,
    setRandomPassword: boolean,
    shouldSetPasswordOnNextLogin: boolean,
    sendActivationEmail: boolean,
    isLockOutEnabled: boolean,
    emailVerificationRequired: boolean,
    roles: Array<ICreateUpdateUserRoleItem>,
    picture: string,
    uploadedPicture?: File
}