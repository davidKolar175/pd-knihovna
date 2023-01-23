import React, { useState } from 'react';
import './Catalog.css';

interface Book {
  id: number;
  title: string;
  author: string;
  pages: number;
  published: string;
  stock: number;
}

interface CatalogProps {
  books: Book[];
  onAddToCart: (book: Book) => void;
  onLogout: () => void;
}

const Catalog: React.FC<CatalogProps> = ({ books, onAddToCart, onLogout }) => {
  return (
    <div>
      <button className="logout-btn" onClick={onLogout}>Log Out</button>
      {books.map(book => (
        <div key={book.id} className="book-container">
          <h3>{book.title}</h3>
          <p>Author: {book.author}</p>
          <p>Pages: {book.pages}</p>
          <p>Published: {book.published}</p>
          <p>In stock: {book.stock}</p>
          <button onClick={() => onAddToCart(book)}>Add to Cart</button>
        </div>
      ))}
    </div>
  );
};

export default Catalog;