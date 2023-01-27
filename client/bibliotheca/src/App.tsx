import './App.css';
import React, { useState } from 'react';
import Login from './Login';
import Catalog from './Catalog';
import { Admin } from './AdminComponent';
import { UserType } from './types';

interface Book {
    id: number;
    title: string;
    author: string;
    pages: number;
    published: string;
    stock: number;
}

const loadBook = async (user: UserType): Promise<Book[]> => {
    const res = await fetch(`https://localhost:7169/api/Books/GetBorrowedBooks?userId=${user.id}`, {  // Enter your IP address here
        method: "GET",
        mode: "cors",
        headers: {
            "Content-type": "text/plain; charset=UTF-8",
            "authorization": `Basic ${btoa(unescape(encodeURIComponent(`${user.userName}:${user.password}`)))}`,
        },
    })

    const resTest = await res.text();
    const temp: any[] = JSON.parse(resTest);
    const books: Book[] = temp.map(x => ({ id: x.Id, author: x.Author, title: x.BookName}) as Book)
    return books;
};

const App: React.FC = () => {
    const [loggedIn, setLoggedIn] = useState(false);
    const [cart, setCart] = useState<Book[]>([]);

    //Just for the sake of example. I want to see some books.
    const [books, setBooks] = useState<Book[]>([]);

    const [isAdmin, setIsAdmin] = useState(false);

    const handleLogin = async (user: UserType) => {
        setLoggedIn(true);
        setIsAdmin(user.isAdmin);
        const test = await loadBook(user);
        setBooks(test);
    };

    const handleLogout = () => {
        setLoggedIn(false);
        setCart([]);
    }

    const handleAddToCart = (book: Book) => {
        setBooks(prevBooks => {
            const updatedBooks = prevBooks.map(b => {
                if (b.id === book.id) {
                    return { ...b, stock: b.stock - 1 }
                }
                return b
            });
            setCart([...cart, book]);
            return updatedBooks;
        });
    };

    return (
        <div>
            {!loggedIn ? (
                <Login onLogin={handleLogin} />
            ) : (
                <>
                    <Catalog books={books} onAddToCart={handleAddToCart} onLogout={handleLogout} />
                    {isAdmin && <Admin />}
                    <div className="cart-container">
                        <h3>Cart</h3>
                        {cart.map(book => (
                            <div key={book.id} className="cart-item">
                                {book.title}
                            </div>
                        ))}
                    </div>
                </>
            )}
        </div>
    );
};

export default App;