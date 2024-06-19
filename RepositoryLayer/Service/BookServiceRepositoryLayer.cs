using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Repository.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class BookServiceRepositoryLayer : IBookRepositoryLayer
    {
        private readonly DapperContext _context;

        public BookServiceRepositoryLayer(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> InsertBookAsync(Book book)
        {
            await EnsureBooksTableCreatedAsync();

            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    book.ImgSrc,
                    book.Title,
                    book.Author,
                    book.Rating,
                    book.RatingPeopleCount,
                    book.Price,
                    book.DiscountedPrice,
                    book.OutOfStock,
                    book.Description
                };

                return await conn.ExecuteScalarAsync<int>("Book_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                return (await conn.QueryAsync<Book>("Book_SelectAll", commandType: CommandType.StoredProcedure)).AsList();
            }
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { BookId = bookId };
                return await conn.QueryFirstOrDefaultAsync<Book>("Book_SelectById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateBookAsync(Book book)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    book.BookId,
                    book.ImgSrc,
                    book.Title,
                    book.Author,
                    book.Rating,
                    book.RatingPeopleCount,
                    book.Price,
                    book.DiscountedPrice,
                    book.OutOfStock,
                    book.Description
                };

                await conn.ExecuteAsync("Book_Update", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteBookAsync(int bookId)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { BookId = bookId };
                await conn.ExecuteAsync("Book_Delete", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        private async Task EnsureBooksTableCreatedAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Books]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Books] (
                            [BookId] INT IDENTITY(1,1) PRIMARY KEY,
                            [ImgSrc] NVARCHAR(255) NOT NULL,
                            [Title] NVARCHAR(255) NOT NULL,
                            [Author] NVARCHAR(255) NOT NULL,
                            [Rating] FLOAT NOT NULL,
                            [RatingPeopleCount] INT NOT NULL,
                            [Price] DECIMAL(18, 2) NOT NULL,
                            [DiscountedPrice] DECIMAL(18, 2) NOT NULL,
                            [OutOfStock] BIT NOT NULL,
                            [Description] NVARCHAR(MAX) NOT NULL
                        );
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Book_Insert')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[Book_Insert]
                                @ImgSrc NVARCHAR(255),
                                @Title NVARCHAR(255),
                                @Author NVARCHAR(255),
                                @Rating FLOAT,
                                @RatingPeopleCount INT,
                                @Price DECIMAL(18, 2),
                                @DiscountedPrice DECIMAL(18, 2),
                                @OutOfStock BIT,
                                @Description NVARCHAR(MAX)
                            AS
                            BEGIN
                                INSERT INTO [dbo].[Books] (ImgSrc,Title, Author, Rating, RatingPeopleCount, Price, DiscountedPrice, OutOfStock, Description)
                                VALUES (@ImgSrc, @Title, @Author, @Rating, @RatingPeopleCount, @Price, @DiscountedPrice, @OutOfStock, @Description);
                                SELECT SCOPE_IDENTITY();
                            END
                        ');
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Book_SelectAll')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[Book_SelectAll]
                            AS
                            BEGIN
                                SELECT * FROM [dbo].[Books];
                            END
                        ');
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Book_SelectById')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[Book_SelectById]
                                @BookId INT
                            AS
                            BEGIN
                                SELECT * FROM [dbo].[Books] WHERE BookId = @BookId;
                            END
                        ');
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Book_Update')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[Book_Update]
                                @BookId INT,
                                @ImgSrc NVARCHAR(255),
                                @Title NVARCHAR(255),
                                @Author NVARCHAR(255),
                                @Rating FLOAT,
                                @RatingPeopleCount INT,
                                @Price DECIMAL(18, 2),
                                @DiscountedPrice DECIMAL(18, 2),
                                @OutOfStock BIT,
                                @Description NVARCHAR(MAX)
                            AS
                            BEGIN
                                UPDATE [dbo].[Books]
                                SET ImgSrc = @ImgSrc,
                                    Title = @Title,
                                    Author = @Author,
                                    Rating = @Rating,
                                    RatingPeopleCount = @RatingPeopleCount,
                                    Price = @Price,
                                    DiscountedPrice = @DiscountedPrice,
                                    OutOfStock = @OutOfStock,
                                    Description = @Description
                                WHERE BookId = @BookId;
                            END
                        ');
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Book_Delete')
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[Book_Delete]
                                @BookId INT
                            AS
                            BEGIN
                                DELETE FROM [dbo].[Books] WHERE BookId = @BookId;
                            END
                        ');
                    END;
                ");
            }
        }
    }
}
