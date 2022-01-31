import { Component, ReactNode } from "react";
import { MainContext } from "../contexts/main-context";

interface IProps {
    requiredPermissions: Array<string>,
    children: ReactNode
}

export default class CheckPermission extends Component<IProps, {}>{
    static contextType = MainContext;

    public _isAllowed = (): boolean => {
        const permissions: Array<string> = this.context.permissions;
        return PermissionHelper.isAllowed(permissions, this.props.requiredPermissions);
    }

    render() {
        return this._isAllowed() ? <>{this.props.children}</> : null;
    }
}

export class PermissionHelper {
    static isAllowed = (grantedPermissions: Array<string>, requiredPermissions: Array<string>): boolean => {
        for (var i = 0; i < requiredPermissions.length; i++) {
            for (var j = 0; j < grantedPermissions.length; j++) {
                if (requiredPermissions[i] === grantedPermissions[j])
                    return true;
            }
        }
        return false;
    }
}