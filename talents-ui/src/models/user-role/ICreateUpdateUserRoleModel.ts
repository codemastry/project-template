export interface ICreateUpdateUserRoleModel {
    id: number,
    name: string,
    isDefault: boolean,
    grantedPermissions: Array<string>
}