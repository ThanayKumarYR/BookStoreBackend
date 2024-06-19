using System.Data;
using Dapper;
using Repository.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserServiceRepositoryLayer : IUserRepositoryLayer
    {
        private readonly DapperContext _context;

        public UserServiceRepositoryLayer(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> InsertUserAsync(string fullName, string emailId, string password, string mobileNumber)
        {
            await EnsureUsersTableCreatedAsync();
            await CreateStoredProceduresAsync();

            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    FullName = fullName,
                    EmailId = emailId,
                    Password = password,
                    MobileNumber = mobileNumber
                };

                return await conn.ExecuteScalarAsync<int>("User_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                return (await conn.QueryAsync<User>("User_SelectAll", commandType: CommandType.StoredProcedure)).AsList();
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { UserId = userId };
                return await conn.QueryFirstOrDefaultAsync<User>("User_SelectById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateUserAsync(int userId, string fullName, string emailId, string password, string mobileNumber)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    UserId = userId,
                    FullName = fullName,
                    EmailId = emailId,
                    Password = password,
                    MobileNumber = mobileNumber
                };

                await conn.ExecuteAsync("User_Update", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { UserId = userId };
                await conn.ExecuteAsync("User_Delete", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        private async Task EnsureUsersTableCreatedAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Users] (
                            [UserId] INT IDENTITY(1,1) PRIMARY KEY,
                            [FullName] NVARCHAR(100) NOT NULL,
                            [EmailId] NVARCHAR(100),
                            [Password] NVARCHAR(100) NOT NULL,
                            [MobileNumber] NVARCHAR(15)
                        );
                    END
                ");
            }
        }

        // Stored Procedure Scripts
        public async Task CreateStoredProceduresAsync()
        {
            await CreateInsertProcedureAsync();
            await CreateSelectAllProcedureAsync();
            await CreateSelectByIdProcedureAsync();
            await CreateUpdateProcedureAsync();
            await CreateDeleteProcedureAsync();
        }

        private async Task CreateInsertProcedureAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Insert]') AND type in (N'P', N'PC'))
                    BEGIN
                        EXEC('
                        CREATE PROCEDURE [dbo].[User_Insert]
                            @FullName NVARCHAR(100),
                            @EmailId NVARCHAR(100),
                            @Password NVARCHAR(100),
                            @MobileNumber NVARCHAR(15)
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            INSERT INTO [dbo].[Users] (FullName, EmailId, Password, MobileNumber)
                            VALUES (@FullName, @EmailId, @Password, @MobileNumber);

                            SELECT SCOPE_IDENTITY() AS UserId;
                        END
                        ');
                    END
                ");
            }
        }

        private async Task CreateSelectAllProcedureAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_SelectAll]') AND type in (N'P', N'PC'))
                    BEGIN
                        EXEC('
                        CREATE PROCEDURE [dbo].[User_SelectAll]
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            SELECT * FROM [dbo].[Users];
                        END
                        ');
                    END
                ");
            }
        }

        private async Task CreateSelectByIdProcedureAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_SelectById]') AND type in (N'P', N'PC'))
                    BEGIN
                        EXEC('
                        CREATE PROCEDURE [dbo].[User_SelectById]
                            @UserId INT
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            SELECT * FROM [dbo].[Users] WHERE UserId = @UserId;
                        END
                        ');
                    END
                ");
            }
        }

        private async Task CreateUpdateProcedureAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Update]') AND type in (N'P', N'PC'))
                    BEGIN
                        EXEC('
                        CREATE PROCEDURE [dbo].[User_Update]
                            @UserId INT,
                            @FullName NVARCHAR(100),
                            @EmailId NVARCHAR(100),
                            @Password NVARCHAR(100),
                            @MobileNumber NVARCHAR(15)
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            UPDATE [dbo].[Users]
                            SET FullName = @FullName,
                                EmailId = @EmailId,
                                Password = @Password,
                                MobileNumber = @MobileNumber
                            WHERE UserId = @UserId;
                        END
                        ');
                    END
                ");
            }
        }

        private async Task CreateDeleteProcedureAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Delete]') AND type in (N'P', N'PC'))
                    BEGIN
                        EXEC('
                        CREATE PROCEDURE [dbo].[User_Delete]
                            @UserId INT
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            DELETE FROM [dbo].[Users] WHERE UserId = @UserId;
                        END
                        ');
                    END
                ");
            }
        }
    }
}
