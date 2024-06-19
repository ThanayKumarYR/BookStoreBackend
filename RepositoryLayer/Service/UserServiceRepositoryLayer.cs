using System.Data;
using Dapper;
using ModelLayer.UserModel;
using Repository.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserServiceRepositoryLayer : IUserRepositoryLayer
    {
        private readonly DapperContext _context;
        private readonly IAuthServiceRepositoryLayer _authService;
        public UserServiceRepositoryLayer(DapperContext context, IAuthServiceRepositoryLayer authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<int> InsertUserAsync(string fullName, string emailId, string password, string mobileNumber)
        {
            if(!await CheckIfUsersTableExistsAsync()) await EnsureUsersTableCreatedAsync();

            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    FullName = fullName,
                    EmailId = emailId,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
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

        public async Task<string> GetUserByEmailIdAsync(UserLoginModel userLoginModel)
        {
            string token = string.Empty;
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { EmailId =  userLoginModel.EmailId };
                User user =  await conn.QueryFirstOrDefaultAsync<User>("User_SelectByEmailId", parameters, commandType: CommandType.StoredProcedure);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                else 
                {
                    if (!BCrypt.Net.BCrypt.Verify(userLoginModel.Password,user.Password)) { 
                        throw new Exception("Invalid Password Credential"); 
                    }
                    token = _authService.GenerateJwtToken(user);
                }
            }

            return token;
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
                            [EmailId] NVARCHAR(100) NOT NULL,
                            [Password] NVARCHAR(100) NOT NULL,
                            [MobileNumber] NVARCHAR(15)
                        );
                    END
                ");
            }

            await CreateStoredProceduresAsync();
        }

        public async Task<bool> CheckIfUsersTableExistsAsync()
        {
            const string query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT * 
                    FROM sys.objects 
                    WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U')
                ) THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT) END";

            using (var conn = _context.CreateConnection())
            {
                return await conn.ExecuteScalarAsync<bool>(query);
            }
        }

        // Stored Procedure Scripts
        public async Task CreateStoredProceduresAsync()
        {
            await CreateInsertProcedureAsync();
            await CreateSelectAllProcedureAsync();
            await CreateSelectByEmailIdProcedureAsync();
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

        private async Task CreateSelectByEmailIdProcedureAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_SelectByEmailId]') AND type in (N'P', N'PC'))
                    BEGIN
                        EXEC('
                        CREATE PROCEDURE [dbo].[User_SelectByEmailId]
                            @EmailId NVARCHAR(100)
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            SELECT * FROM [dbo].[Users] WHERE EmailId = @EmailId;
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
