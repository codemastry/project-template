import { InfoCircleFilled, SaveFilled } from "@ant-design/icons";
import { Alert, Button, Checkbox, Form, Input, message, Modal, Skeleton, Tooltip, Tree } from "antd";
import React, { Component } from "react";
import { IAlertModel } from "../../models/IAlertModel";
import { ICreateUpdateUserRoleModel } from "../../models/user-role/ICreateUpdateUserRoleModel";
import UserRoleService from "../../services/user-role-service";

interface IProps {
    show: boolean,
    onClose: () => void,
    onOk: () => void,
    onSuccess: () => void
}

interface IState {
    isLoading: boolean,
    alert: IAlertModel,
    isLoadingTree: boolean,
    allPermissions: any,
    selectedPermissions: Array<string>,
    showSpinner: boolean
}

export default class CreateRoleModal extends Component<IProps, IState>{
    state = {
        showSpinner: false,
        isLoading: false,
        alert: {
            show: false,
            message: '',
            isSuccess: false
        },
        isLoadingTree: false,
        allPermissions: [],
        selectedPermissions: []
    }

    componentDidMount = async () => {
        await this._loadData();
    }

    _loadData = async () => {
        this.setState({ isLoading: true, isLoadingTree: true, showSpinner: true });
        try {
            const allPermissions = await UserRoleService.getAllPermissionsForTreeView();
            this.setState({ allPermissions });
        }
        catch (error) {
            message.error("An error occured");
        }
        this.setState({ isLoading: false, isLoadingTree: false, showSpinner: false });
    }

    _onFinish = async (values: any) => {
        this.setState({ isLoading: true });
        try {
            var data: ICreateUpdateUserRoleModel = {
                grantedPermissions: this.state.selectedPermissions,
                id: 0,
                isDefault: values.isDefault,
                name: values.name
            };
            const result = await UserRoleService.create(data);
            if (result.isSuccess) {
                message.success("User role added.");
                // this.setState({ alert: { show: true, message: `User role successfully added`, isSuccess: true } });
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

    _selectPermissions = async (selectedPermissions: any) => {
        this.setState({ selectedPermissions });
    }

    render() {
        return (
            <>
                <Modal
                    maskClosable={false}
                    title="Create new role"
                    visible={this.props.show}
                    onOk={this.props.onOk}
                    onCancel={this.props.onClose}
                    footer={
                        [
                            <Button key="back" onClick={this.props.onClose}>
                                Cancel
                            </Button>,
                            <Button icon={<SaveFilled />} form="createRoleForm" htmlType="submit" key="submit" type="primary" loading={this.state.isLoading} onClick={this.props.onOk}>
                                Save
                            </Button>,
                        ]
                    }>

                    {this.state.showSpinner ?
                        <Skeleton active /> :
                        <fieldset disabled={this.state.isLoading}>
                            <Form
                                id="createRoleForm"
                                layout="vertical"
                                onFinish={this._onFinish}>
                                {this.state.alert.show && <Alert type={this.state.alert.isSuccess ? 'success' : 'error'} message={this.state.alert.message} />}
                                <p></p>
                                <Form.Item
                                    label="Name"
                                    name="name"
                                    rules={[{ required: true, message: 'Please provide role name' }]}>
                                    <Input />
                                </Form.Item>

                                <Form.Item name="isDefault" valuePropName="checked">
                                    <Checkbox>
                                        Default
                                        <Tooltip title="When adding a new user, this role will be checked by default."><InfoCircleFilled className="info-icon-tooltip" /></Tooltip>
                                    </Checkbox>
                                </Form.Item>

                                <Form.Item
                                    label="Permissions">
                                    {!this.state.isLoadingTree &&
                                        <Tree
                                            showLine
                                            checkable
                                            defaultExpandAll
                                            selectedKeys={this.state.selectedPermissions}
                                            checkedKeys={this.state.selectedPermissions}
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