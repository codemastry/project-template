export interface IUserRoleModel {
    id: number,
    name: string,
    companyId: number,
    isDefault: boolean,
    dateCreated: Date,
    permission: string,
    permissionList: Array<string>
}