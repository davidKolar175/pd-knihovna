import React, { useState } from 'react';
import './Login.css';

const httpGet = () => {
  var xmlHttp = new XMLHttpRequest();
  xmlHttp.open( "GET", "https://localhost:7169/api/users", false );
  xmlHttp.setRequestHeader("authorization", "Basic aGFoYTphc2Rn");
  xmlHttp.send(null);
  return xmlHttp.responseText;
}

interface LoginProps {
  onLogin: (username: string, password: string) => void;
}

const Login: React.FC<LoginProps> = ({ onLogin }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    console.log(httpGet());
    onLogin(username, password);
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
    </form>
  );
};

export default Login;