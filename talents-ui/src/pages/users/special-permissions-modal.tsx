import { InfoCircleFilled, SaveFilled } from "@ant-design/icons";
import { Alert, Button, Checkbox, Form, FormInstance, Input, message, Modal, Skeleton, Tooltip, Tree } from "antd";
import React, { Component } from "react";
import { IAlertModel } from "../../models/IAlertModel";
import { ICreateUpdateUserRoleModel } from "../../models/user-role/ICreateUpdateUserRoleModel";
import { IUserRoleModel } from "../../models/user-role/IUserRoleModel";
import UserRoleService from "../../services/user-role-service";
import UserService from "../../services/user-service";

interface IProps {
    id: number,
    show: boolean,
    onClose: () => void,
    onOk: () => void,
    onSuccess: () => void
}

interface IState {
    isLoading: boolean,
    isLoadingTree: boolean,
    alert: IAlertModel,
    grantedPermissions: Array<string>,
    allPermissions: any,
    showSpinner: boolean
}

export default class SpecialPermissionsModal extends Component<IProps, IState>{
    formRef = React.createRef<FormInstance>();
    state = {
        showSpinner: false,
        isLoading: false,
        isLoadingTree: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        },
        grantedPermissions: [],
        allPermissions: []
    }

    componentDidMount = async () => {
        await this._loadData();
    }

    _loadData = async () => {
        this.setState({ isLoading: true, isLoadingTree: true, showSpinner: true });
        try {
            const grantedPermissions = await UserService.getSpecialPermissions(this.props.id);
            const allPermissions = await UserRoleService.getAllPermissionsForTreeView();
            this.setState({ allPermissions, grantedPermissions: grantedPermissions.data, showSpinner: false });
        }
        catch (error) {
            message.error("An error occured");
        }
        this.setState({ isLoading: false, isLoadingTree: false });
    }

    _onFinish = async (values: any) => {
        this.setState({ isLoading: true });
        try {
            const id = this.props.id;
            const grantedPermissions = this.state.grantedPermissions;
            const result = await UserService.saveSpecialPermissions(id, grantedPermissions);
            if (result.isSuccess) {
                message.success("User permissions updated.");
                this.props.onSuccess();
            }
            else {
                message.error(result.message);
                this.setState({ alert: { show: true, message: result.message, isSuccess: false } });
            }
        }
        catch (error) {
            message.error("An error occured");
        }
        this.setState({ isLoading: false });
    }

    _selectPermissions = async (permissionList: any) => {
        this.setState({ grantedPermissions: permissionList });
    }

    render() {
        return (
            <>
                <Modal
                    maskClosable={false}
                    title="Special permissions"
                    visible={this.props.show}
                    onOk={this.props.onOk}
                    onCancel={this.props.onClose}
                    footer={
                        [
                            <Button key="back" onClick={this.props.onClose}>
                                Cancel
                            </Button>,
                            <Button icon={<SaveFilled />} form="editSpecialPermissionsForm" htmlType="submit" key="submit" type="primary" loading={this.state.isLoading} onClick={this.props.onOk}>
                                Save changes
                            </Button>,
                        ]
                    }>

                    {this.state.showSpinner ?
                        <Skeleton active /> :
                        <fieldset disabled={this.state.isLoading}>
                            <Form
                                ref={this.formRef}
                                id="editSpecialPermissionsForm"
                                layout="vertical"
                                onFinish={this._onFinish}>
                                {this.state.alert.show && <Alert type={this.state.alert.isSuccess ? 'success' : 'error'} message={this.state.alert.message} />}
                                <p></p>
                                <Form.Item
                                    label="Permissions">
                                    {!this.state.isLoadingTree &&
                                        <Tree
                                            showLine
                                            checkable
                                            defaultExpandAll
                                            selectedKeys={this.state.grantedPermissions}
                                            checkedKeys={this.state.grantedPermissions}
                                            onCheck={this._selectPermissions}
                                            treeData={this.state.allPermissions}
                                        />
                                    }
                                </Form.Item>
                            </Form>
                        </fieldset>
                    }
                </Modal>
            </>
        )
    }
}