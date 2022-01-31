import React, { Component } from "react";
import moment from "moment";
import { Button, Divider, Dropdown, Menu, message, Modal, Space, Spin, Table, Tag } from "antd";
import { DeleteFilled, DownOutlined, EditFilled, LockFilled, LoginOutlined, PlusOutlined, SolutionOutlined, UnlockFilled } from "@ant-design/icons";
import "./style.css";
import { DATETIME_FORMAT, LOCALSTORAGE, PERMISSIONS } from "../../models/constants";
import Search from "antd/lib/input/Search";
import EditUserModal from "./edit-user-modal";
import CreateUserModal from "./create-user-modal";
import { IUserModel } from "../../models/user/IUserModel";
import UserService from "../../services/user-service";
import SpecialPermissionsModal from "./special-permissions-modal";
import CheckPermission, { PermissionHelper } from "../../helpers/permission-helper";
import { MainContext } from "../../contexts/main-context";

interface IState {
    isLoading: boolean,
    data: Array<IUserModel>,
    showAdd: boolean,
    showEdit: boolean,
    showSpecialPermissions: boolean,
    selectedId: number
}
export default class Users extends Component<{}, IState> {
    static contextType = MainContext;

    _isAllowed = (requiredPermissions: Array<string>): boolean => {
        const permissions: Array<string> = this.context.permissions;
        return PermissionHelper.isAllowed(permissions, requiredPermissions);
    }

    state = {
        isLoading: false,
        data: [],
        showAdd: false,
        showEdit: false,
        showSpecialPermissions: false,
        selectedId: 0
    }

    columns = [
        {
            title: 'Name',
            dataIndex: 'name',
            key: 'name',
            render: (text: string, row: IUserModel) => <Button type="link" onClick={() => this._toggleEdit(true, row.id)}>{row.firstName} {row.lastName}</Button>,
        },
        {
            title: 'Date created',
            dataIndex: 'dateCreated',
            key: 'dateCreated',
            render: (text: Date) => moment(text).format(DATETIME_FORMAT),
        },
        {
            title: 'Roles',
            dataIndex: 'roles',
            key: 'roles',
            render: (text: Array<string>) => text.map(t => <Tag color="blue" key={t}>{t}</Tag>),
        },
        {
            title: 'Email verified',
            dataIndex: 'isEmailVerified',
            key: 'isEmailVerified',
            render: (text: boolean) => text ? <Tag color="green">Yes</Tag> : <Tag color="red">No</Tag>,
        },
        {
            title: 'Action',
            key: '',
            dataIndex: '',
            render: (data: any, row: IUserModel) =>
                this._isAllowed([
                    PERMISSIONS.UsersDelete,
                    PERMISSIONS.UsersImpersonate,
                    PERMISSIONS.UsersLock,
                    PERMISSIONS.UsersSpecialPermission,
                    PERMISSIONS.UsersUpdate
                ]) &&
                <Dropdown
                    overlay={
                        <>
                            <Menu className="drop-down">
                                {this._isAllowed([PERMISSIONS.UsersImpersonate]) &&
                                    <Menu.Item key="loginasuser">
                                        <Button onClick={async () => await this._handleLoginAsUserClick(row)} type="link" icon={<LoginOutlined />}>Login as this user</Button>
                                    </Menu.Item>
                                }

                                {this._isAllowed([PERMISSIONS.UsersUpdate]) &&
                                    <Menu.Item key="0">
                                        <Button onClick={() => this._toggleEdit(true, row.id)} type="link" icon={<EditFilled />}>Edit</Button>
                                    </Menu.Item>
                                }

                                {this._isAllowed([PERMISSIONS.UsersSpecialPermission]) &&
                                    <Menu.Item key="2">
                                        <Button onClick={() => this._toggleSpecialPermissions(true, row.id)} type="link" icon={<SolutionOutlined />}>Special Permissions</Button>
                                    </Menu.Item>
                                }

                                {this._isAllowed([PERMISSIONS.UsersLock]) &&
                                    <Menu.Item key="lockunlock">
                                        <Button onClick={() => this._toggleLockUnlock(row.id, !row.isLocked)} type="link" icon={row.isLocked ? <UnlockFilled /> : <LockFilled />}>{row.isLocked ? 'Unlock' : 'Lock'}</Button>
                                    </Menu.Item>
                                }

                                {this._isAllowed([PERMISSIONS.UsersDelete]) &&
                                    <Menu.Item key="1">
                                        <Button onClick={() => this._delete(row.id)} type="link" danger icon={<DeleteFilled />}>Delete</Button>
                                    </Menu.Item>
                                }

                            </Menu>
                        </>
                    }
                    trigger={['click']}>
                    <a className="ant-dropdown-link" onClick={e => e.preventDefault()}>
                        Action <DownOutlined />
                    </a>
                </Dropdown>
        },
    ]

    componentDidMount = async () => await this._loadData();

    _loadData = async (search?: string) => {
        this.setState({ isLoading: true });
        const data = await UserService.getAll(search);
        this.setState({ data, isLoading: false });
    }

    _toggleEdit = async (showEdit: boolean, selectedId: number) => this.setState({ showEdit, selectedId })
    _toggleAdd = async (showAdd: boolean) => this.setState({ showAdd });
    _toggleSpecialPermissions = async (showSpecialPermissions: boolean, selectedId: number) => this.setState({ showSpecialPermissions, selectedId });
    _onSearch = async (str: string) => await this._loadData(str);

    _onCretionSuccess = async () => {
        this._toggleAdd(false);
        await this._loadData();
    }
    _onEditSuccess = async () => {
        this._toggleEdit(false, 0);
        await this._loadData();
    }
    _onSpecialPermissionSuccess = async () => {
        this._toggleSpecialPermissions(false, 0);
        await this._loadData();
    }

    _delete = (id: number) => {
        Modal.confirm({
            title: "Delete?",
            content: this.state.isLoading ?
                <div className="text-center"><Spin /></div> :
                <><p>Are you sure? This action is permanent.</p></>,
            onOk: async () => {
                this.state.isLoading = true;
                const result = await UserService.delete(id);
                if (result.isSuccess) await this._loadData();
                else message.error(result.message);
                this.state.isLoading = false;
            },
            okButtonProps: { loading: this.state.isLoading },
            cancelButtonProps: { loading: this.state.isLoading },
            keyboard: false
        });
    }

    _toggleLockUnlock = (userId: number, isLocked: boolean) => {
        Modal.confirm({
            title: "Confirm",
            content: this.state.isLoading ?
                <div className="text-center"><Spin /></div> :
                <><p>{isLocked ? 'Lock' : 'Unlock'} this user account?</p></>,
            onOk: async () => {
                this.state.isLoading = true;
                const result = await UserService.toggleLock(userId, isLocked);
                if (result.isSuccess) await this._loadData();
                else message.error(result.message);
                this.state.isLoading = false;
            },
            okButtonProps: { loading: this.state.isLoading },
            cancelButtonProps: { loading: this.state.isLoading },
            keyboard: false
        });
    }

    _handleLoginAsUserClick = async (user: IUserModel) => {
        Modal.confirm({
            title: "Confirm",
            content: this.state.isLoading ?
                <div className="text-center"><Spin /></div> :
                <><p>Login as {`${user.firstName} ${user.lastName}`}</p></>,
            onOk: async () => await this._impersonate(user.id),
            okButtonProps: { loading: this.state.isLoading },
            cancelButtonProps: { loading: this.state.isLoading },
            keyboard: false
        });
    }

    _impersonate = async (id: number) => {
        try {
            const result = await UserService.impersonate(id);
            if (result.isSuccess) {
                message.success("Redirecting you to dashboard.");
                localStorage.setItem(LOCALSTORAGE.TOKEN, result.data);
                window.location.href = '/dashboard';
            }
            else {
                message.error(result.message);
            }
        }
        catch (error) {
            message.error("An error occured while processing request");
        }
    }
    render() {
        return (
            <>
                <h3>Users</h3>

                <Search placeholder="Name, Email" onSearch={this._onSearch} enterButton allowClear />
                <div style={{ height: 25 }}></div>
                <CheckPermission requiredPermissions={[PERMISSIONS.UsersCreate]}>
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => this._toggleAdd(true)}>Add new</Button>
                </CheckPermission>
                <p></p>
                <Table
                    loading={this.state.isLoading}
                    columns={this.columns}
                    rowKey="id"
                    dataSource={this.state.data} />

                {this.state.showAdd &&
                    <CreateUserModal
                        onClose={() => this._toggleAdd(false)}
                        onOk={() => { }}
                        show={this.state.showAdd}
                        onSuccess={this._onCretionSuccess}
                    />}

                {this.state.showEdit &&
                    <EditUserModal
                        id={this.state.selectedId}
                        onClose={() => this._toggleEdit(false, 0)}
                        onOk={() => { }}
                        show={this.state.showEdit}
                        onSuccess={this._onEditSuccess}
                    />}

                {this.state.showSpecialPermissions &&
                    <SpecialPermissionsModal
                        id={this.state.selectedId}
                        onClose={() => this._toggleSpecialPermissions(false, 0)}
                        onOk={() => { }}
                        show={this.state.showSpecialPermissions}
                        onSuccess={this._onSpecialPermissionSuccess}
                    />}
            </>
        );
    }
}
