using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IBookBusinessLayer
    {
        Task<int> AddBookAsync(Book book);
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int bookId);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int bookId);
    }
}
