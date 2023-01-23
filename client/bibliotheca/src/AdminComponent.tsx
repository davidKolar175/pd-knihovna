import React, { useState } from 'react';
import "./AdminComponent.css";

interface User {
    id: number;
    username: string;
    password: string;
    banned: boolean;
}

export const Admin: React.FC = () => {
    const [users, setUsers] = useState<User[]>([
        { id: 1, username: "user1", password: "password1", banned: false },
        { id: 2, username: "user2", password: "password2", banned: false },
        { id: 3, username: "user3", password: "password3", banned: false }
    ]);

    const handleBanUser = (id: number) => {
        setUsers(prevUsers => {
            const updatedUsers = prevUsers.map(user => {
                if (user.id === id) {
                    return { ...user, banned: true }
                }
                return user
            });
            return updatedUsers;
        });
    };

    const handleUnbanUser = (id: number) => {
        setUsers(prevUsers => {
            const updatedUsers = prevUsers.map(user => {
                if (user.id === id) {
                    return { ...user, banned: false }
                }
                return user
            });
            return updatedUsers;
        });
    };


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
                            <td>{user.username}</td>
                            <td>{user.banned ? "Yes" : "No"}</td>
                            <td>
                                <button onClick={() => handleBanUser(user.id)} disabled={user.banned}>
                                    Ban User
                                </button>
                                <button onClick={() => handleUnbanUser(user.id)} disabled={!user.banned}>
                                    Unban User
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

