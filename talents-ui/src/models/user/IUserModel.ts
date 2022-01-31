import { ICreateUpdateUserRoleItem } from "./ICreateUpdateUserRoleItem";

export interface IUserModel {
    id: number,
    firstName: string,
    lastName: string,
    email: string,
    isActive: boolean,
    isLocked: boolean,
    shouldSetPasswordOnNextLogin: boolean,
    isLockOutEnabled: boolean,
    roles: Array<ICreateUpdateUserRoleItem>,
    isEmailVerified: boolean,
    picture?: string
}