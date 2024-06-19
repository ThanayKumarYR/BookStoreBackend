using BusinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class BookServiceBusinessLayer : IBookBusinessLayer
    {
        private readonly IBookRepositoryLayer _bookRepositoryLayer;

        public BookServiceBusinessLayer(IBookRepositoryLayer bookRepositoryLayer)
        {
            _bookRepositoryLayer = bookRepositoryLayer;
        }

        public async Task<int> AddBookAsync(Book book)
        {
            return await _bookRepositoryLayer.InsertBookAsync(book);
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _bookRepositoryLayer.GetAllBooksAsync();
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            return await _bookRepositoryLayer.GetBookByIdAsync(bookId);
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _bookRepositoryLayer.UpdateBookAsync(book);
        }

        public async Task DeleteBookAsync(int bookId)
        {
            await _bookRepositoryLayer.DeleteBookAsync(bookId);
        }
    }
}
