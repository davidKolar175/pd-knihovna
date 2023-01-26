import React, { useState } from "react";
import "./Login.css";

const postLogin = async (userName: string, password: string): Promise<boolean> => {
    // const hehe = Buffer.from(`${userName}:${password}`).toString("base64");

    const res = await fetch("https://localhost:7169/api/login", {  // Enter your IP address here
        method: "POST",
        mode: "cors",
        headers: {
            "Content-type": "text/plain; charset=UTF-8",
            "authorization": `Basic ${btoa(unescape(encodeURIComponent(`${userName}:${password}`)))}`,
        },
    })

    if (res.ok)
        return true;

    return false;
}

interface LoginProps {
    onLogin: (success: boolean, isAdmin: boolean) => void;
}

const Login: React.FC<LoginProps> = ({ onLogin }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (await postLogin(username, password)) {
            onLogin(true, true);
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