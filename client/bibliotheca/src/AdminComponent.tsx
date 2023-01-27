import React, { useEffect, useState } from 'react';
import "./AdminComponent.css";
import { UserType } from './types';


const loadUnauthorizedUsers = async (user: UserType): Promise<UserType[]> => {
    const res = await fetch(`https://localhost:7169/api/Users/GetUnauthorizedUsers`, {  // Enter your IP address here
        method: "GET",
        mode: "cors",
        headers: {
            "Content-type": "text/plain; charset=UTF-8",
            "authorization": `Basic ${btoa(unescape(encodeURIComponent(`${user.userName}:${user.password}`)))}`,
        },
    })

    const resTest = await res.text();
    const temp: any[] = JSON.parse(resTest);
    const books: UserType[] = temp.map(x => ({ id: x.Id, userName: x.UserName}) as UserType)
    return books;
};

export const Admin: React.FC<{user: UserType}> = ({user}) => {
    const [users, setUsers] = useState<UserType[]>([]);

    useEffect(() => {
        loadUnauthorizedUsers(user).then(x => setUsers(x));
    }, [user])


    return (
        <div className="admin-container">
            <h2>Admin</h2>
            <table className="admin-table">
                <thead>
                    <tr>
                        <th>Username</th>
                        <th>Banned</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.id}>
                            <td>{user.userName}</td>
                            <td>{"Unauthorized"}</td>
                            <td>
                                <button>
                                    Authorize user
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

