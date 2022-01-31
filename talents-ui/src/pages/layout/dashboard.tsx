import React, { Component } from "react";
import "./style.css";
import { Layout, Menu } from "antd";
import {
    DashboardOutlined,
    LogoutOutlined,
    SettingOutlined,
    SolutionOutlined,
    TeamOutlined,
} from "@ant-design/icons";
import { Link, RouteComponentProps } from "react-router-dom";
import { LOCALSTORAGE, PERMISSIONS } from "../../models/constants";
import AccountService from "../../services/account-service";
import CheckPermission, { PermissionHelper } from "../../helpers/permission-helper";
import { MainContext } from "../../contexts/main-context";

interface IState {
    collapsed: boolean,
    theme: string
}

export default class DashboardLayout extends Component<RouteComponentProps, IState>{
    static contextType = MainContext;

    _isAllowed = (requiredPermissions: Array<string>): boolean => {
        const permissions: Array<string> = this.context.permissions;
        return PermissionHelper.isAllowed(permissions, requiredPermissions);
    }

    state = {
        collapsed: localStorage.getItem(LOCALSTORAGE.IS_MOBILE_APP) !== null,
        theme: 'light'
    }

    componentDidMount = async () => {
        const isAuthenticated = AccountService.checkAuth();
    }

    _onCollapse = (collapsed: boolean) => this.setState({ collapsed })

    _handleLogout = (e: any) => {
        e.preventDefault();
        localStorage.clear();
        window.location.href = '/login';
    }

    render() {
        return (
            <>
                <Layout id="user-dashboard" style={{ minHeight: '100vh' }}>
                    <Layout.Sider theme={this.state.theme === 'light' ? "light" : "dark"} collapsible collapsed={this.state.collapsed} onCollapse={this._onCollapse}>
                        <div className="logo">
                            <Link to="/">
                                <img src="https://via.placeholder.com/150" /><span>Talents</span>
                            </Link>
                        </div>
                        <Menu theme={this.state.theme === 'light' ? "light" : "dark"} mode="inline">

                            {this._isAllowed([PERMISSIONS.Dashboard]) &&
                                <Menu.Item key="1" icon={<DashboardOutlined />}>
                                    <Link to="/dashboard">Dashboard</Link>
                                </Menu.Item>
                            }

                            {this._isAllowed([PERMISSIONS.Administration]) &&
                                <Menu.SubMenu key="admin" icon={<SettingOutlined />} title="Administration">

                                    {this._isAllowed([PERMISSIONS.Users]) &&
                                        <Menu.Item key="admin-users" icon={<TeamOutlined />}>
                                            <Link to="/users">Users</Link>
                                        </Menu.Item>
                                    }

                                    {this._isAllowed([PERMISSIONS.Roles]) &&
                                        <Menu.Item key="admin-roles" icon={<SolutionOutlined />}>
                                            <Link to="/roles">Roles</Link>
                                        </Menu.Item>
                                    }

                                </Menu.SubMenu>
                            }


                            <Menu.Item key="logout" icon={<LogoutOutlined />}>
                                <a href="#" onClick={this._handleLogout}>Logout</a>
                            </Menu.Item>
                        </Menu>
                    </Layout.Sider>
                    <Layout className="site-layout">
                        <Layout.Content style={{ margin: '0 16px' }}>
                            <div className="site-layout-background" style={{ padding: 24, minHeight: 360 }}>
                                {this.props.children}
                            </div>
                        </Layout.Content>
                        <Layout.Footer style={{ textAlign: 'center' }}>&copy; Talents 2020</Layout.Footer>
                    </Layout>
                </Layout>
            </>
        )
    }
}