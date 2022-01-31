import { Button } from "antd"
import React, { Component } from "react"
import { Link } from "react-router-dom"

export default class LandingPage extends Component {
    render() {
        return (
            <>
                <div style={{ textAlign: 'center' }}>
                    <h1>Liminismo</h1>
                    <h2>Landing page - Carlo Lim</h2>
                    <h3>Talents Boilerplate / Starting project</h3>
                    <p>
                        <Button size="large" type="primary">
                            <Link to="/login">Login</Link>
                        </Button>
                    </p>
                    <p>
                        <Button size="large" type="primary">
                            <Link to="/register">Register</Link>
                        </Button>
                    </p>
                </div>
            </>
        )
    }
}