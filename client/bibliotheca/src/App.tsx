import logo from './logo.svg';
import './App.css';
import React, { useState } from 'react';
import Login from './Login';
import Catalog from './Catalog';
import { Admin } from './AdminComponent';

interface Book {
  id: number;
  title: string;
  author: string;
  pages: number;
  published: string;
  stock: number;
}

const App: React.FC = () => {
  const [loggedIn, setLoggedIn] = useState(false);
  const [cart, setCart] = useState<Book[]>([]);
  //Just for the sake of example. I want to see some books.
  const [books, setBooks] = useState<Book[]>([
    { id: 1, title: "Harry Potter and the Philosopher's Stone", author: "J.K. Rowling", pages: 223, published: "1997", stock: 10 },
    { id: 2, title: "The Lord of the Rings", author: "J.R.R. Tolkien", pages: 1178, published: "1954", stock: 5 },
    { id: 3, title: "The Hobbit", author: "J.R.R. Tolkien", pages: 310, published: "1937", stock: 8 }
  ]);
  const [isAdmin, setIsAdmin] = useState(false);
  const users = [
    { id: 1, username: "user1", password: "password1", role: "basic" }, 
    { id: 2, username: "admin", password: "admin", role: "admin" }, 
    { id: 3, username: "user2", password: "password2", role: "basic" },
  ];

  const handleLogin = (username: string, password: string) => {
    const user = users.find(user => user.username === username && user.password === password);
    if(user){
      setLoggedIn(true);
      if(user.role === "admin"){
        setIsAdmin(true);
      }
    }
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