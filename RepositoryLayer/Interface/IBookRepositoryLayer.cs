using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IBookRepositoryLayer
    {
        Task<int> InsertBookAsync(Book book);
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int bookId);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int bookId);
    }
}
