import React, { Component } from 'react';
import './App.less';
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import Login from './pages/login';
import Register from './pages/register';
import ForgotPassword from './pages/forgotpassword';
import ResetPassword from './pages/resetpassword';
import Dashboard from './pages/dashboard';
import DashboardLayout from './pages/layout/dashboard';
import Roles from "./pages/roles";
import VerifyEmail from './pages/verifyemail';
import ResendVerification from './pages/verifyemail/resend';
import Users from './pages/users';
import UserService from './services/user-service';
import { MainContext } from './contexts/main-context';
import LandingPage from './pages/_landing-page';

interface IState {
  permissions: Array<string>
}

export default class App extends Component<{}, IState> {
  state = {
    permissions: [],
  }

  componentDidMount = async () => {
    const permissions = await UserService.getCurrentUserPermissions();
    this.setState({ permissions });
  }
  render() {
    return (
      <MainContext.Provider value={this.state}>
        <Router>
          <Switch>
            <Route exact path="/" component={LandingPage} />
            <Route exact path="/login" component={Login} />
            <Route exact path="/register" component={Register} />
            <Route exact path="/emailverification/:token" component={VerifyEmail} />
            <Route exact path="/resendverification" component={ResendVerification} />
            <Route exact path="/forgotpassword" component={ForgotPassword} />
            <Route exact path="/resetpassword/:token" component={ResetPassword} />

            <Route exact path="/dashboard" render={(p) =>
              <DashboardLayout {...p}>
                <Dashboard {...p} />
              </DashboardLayout>} />

            <Route exact path="/roles" render={(p) =>
              <DashboardLayout {...p}>
                <Roles {...p} />
              </DashboardLayout>} />

            <Route exact path="/users" render={(p) =>
              <DashboardLayout {...p}>
                <Users {...p} />
              </DashboardLayout>} />
          </Switch>
        </Router>
      </MainContext.Provider>
    );
  }
}