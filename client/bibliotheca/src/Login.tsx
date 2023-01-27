import React, { useState } from "react";
import "./Login.css";
import { UserType } from "./types";

const postLogin = async (userName: string, password: string): Promise<UserType | false> => {
    // const hehe = Buffer.from(`${userName}:${password}`).toString("base64");

    const res = await fetch("https://localhost:7169/api/login", {  // Enter your IP address here
        method: "POST",
        mode: "cors",
        headers: {
            "Content-type": "text/plain; charset=UTF-8",
            "authorization": `Basic ${btoa(unescape(encodeURIComponent(`${userName}:${password}`)))}`,
        },
    })

    if (!res.ok)
        return false;

    const resTest = await res.text();
    const temp = JSON.parse(resTest);
   
    return { id: temp.Id, isAdmin: temp.IsAdmin, userName: temp.UserName, password: password };
}

interface LoginProps {
    onLogin: (user: UserType) => void;
}

const Login: React.FC<LoginProps> = ({ onLogin }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const loginResult = await postLogin(username, password);
        if (loginResult !== false) {
            onLogin(loginResult);
        }
    };

    return (
        <form className="login-form" onSubmit={handleSubmit}>
            <label>
                Username:
                <input type="text" value={username} onChange={e => setUsername(e.target.value)} />
            </label>
            <br />
            <label>
                Password:
                <input type="password" value={password} onChange={e => setPassword(e.target.value)} />
            </label>
            <br />
            <button type="submit">Login</button>
            <button style={{ marginTop: 6 }}>Register</button>
        </form>
    );
};

export default Login;