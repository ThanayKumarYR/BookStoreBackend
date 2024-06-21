using Dapper;
using ModelLayer.CartModel;
using Repository.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System.Data;

namespace RepositoryLayer.Service
{
    public class CartServiceRepositoryLayer:ICartRepositoryLayer
    {
        private readonly DapperContext _context;

        public CartServiceRepositoryLayer(DapperContext context)
        {
            _context = context;
            EnsureStoredProceduresExist().Wait();
        }

        private async Task EnsureStoredProceduresExist()
        {
            using (var connection = _context.CreateConnection())
            {
                var createTableQuery = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cart]') AND type in (N'U'))
BEGIN
    CREATE TABLE Cart (
        CartId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        BookId INT NOT NULL,
        Quantity INT NOT NULL,
        IsWishlist BIT DEFAULT 0
    );
END";

                var createAddToCartProc = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAddToCart]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE spAddToCart
        @UserId INT,
        @BookId INT,
        @Quantity INT,
        @IsWishlist BIT
    AS
    BEGIN
        INSERT INTO Cart (UserId, BookId, Quantity, IsWishlist) 
        VALUES (@UserId, @BookId, @Quantity, @IsWishlist);
        SELECT CAST(SCOPE_IDENTITY() as int);
    END')
END";

                var createGetCartByUserIdProc = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGetCartByUserId]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE spGetCartByUserId
        @UserId INT
    AS
    BEGIN
        SELECT c.*, b.* 
        FROM Cart AS c
        INNER JOIN Books AS b ON c.BookId = b.BookId
        WHERE c.UserId = @UserId;
    END')
END
";

                var createUnCartProc = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUnCart]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE spUnCart
        @CartId INT,
        @UserId INT
    AS
    BEGIN
        DELETE FROM Cart WHERE CartId = @CartId AND UserId = @UserId;
    END')
END";

                var createUpdateCartQuantityProc = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUpdateCartQuantity]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE spUpdateCartQuantity
        @CartId INT,
        @Quantity INT
    AS
    BEGIN
        UPDATE Cart SET Quantity = @Quantity WHERE CartId = @CartId;
    END')
END";

                await connection.ExecuteAsync(createTableQuery);
                await connection.ExecuteAsync(createAddToCartProc);
                await connection.ExecuteAsync(createGetCartByUserIdProc);
                await connection.ExecuteAsync(createUnCartProc);
                await connection.ExecuteAsync(createUpdateCartQuantityProc);
            }
        }

        public async Task<int> AddToCart(Cart model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Quantity", model.Quantity, DbType.Int32);
            parameters.Add("UserId", model.UserId, DbType.Int32);
            parameters.Add("BookId", model.BookId, DbType.Int32);
            parameters.Add("IsWishlist", model.IsWishlist, DbType.Boolean);

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>("spAddToCart", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<CartResponse>> GetCartByUserId(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<CartResponse>("spGetCartByUserId", new { UserId = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task RemoveFromCart(int cartId, int userId)
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spUnCart", new { CartId = cartId, UserId = userId }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateCartQuantity(int cartId, int quantity)
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spUpdateCartQuantity", new { CartId = cartId, Quantity = quantity }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
