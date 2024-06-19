using System.Data;
using Dapper;
using Repository.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserAddressServiceRepositoryLayer : IUserAddressRepositoryLayer
    {
        private readonly DapperContext _context;

        public UserAddressServiceRepositoryLayer(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> InsertAddressAsync(UserAddress userAddress)
        {
            await EnsureUserAddressesTableCreatedAsync();

            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    userAddress.Address,
                    userAddress.City,
                    userAddress.State,
                    userAddress.Type,
                    userAddress.UserId
                };

                return await conn.ExecuteScalarAsync<int>("UserAddress_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<UserAddress>> GetAddressesAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                return (await conn.QueryAsync<UserAddress>("UserAddress_SelectAll", commandType: CommandType.StoredProcedure)).AsList();
            }
        }

        public async Task<UserAddress> GetAddressByIdAsync(int addressId)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { AddressId = addressId };
                return await conn.QueryFirstOrDefaultAsync<UserAddress>("UserAddress_SelectById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAddressAsync(UserAddress userAddress)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new
                {
                    userAddress.AddressId,
                    userAddress.Address,
                    userAddress.City,
                    userAddress.State,
                    userAddress.Type,
                    userAddress.UserId
                };

                await conn.ExecuteAsync("UserAddress_Update", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAddressAsync(int addressId)
        {
            using (var conn = _context.CreateConnection())
            {
                var parameters = new { AddressId = addressId };
                await conn.ExecuteAsync("UserAddress_Delete", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        private async Task EnsureUserAddressesTableCreatedAsync()
        {
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAddresses]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[UserAddresses] (
                            [AddressId] INT IDENTITY(1,1) PRIMARY KEY,
                            [Address] NVARCHAR(255) NOT NULL,
                            [City] NVARCHAR(100) NOT NULL,
                            [State] NVARCHAR(100) NOT NULL,
                            [Type] NVARCHAR(50) NOT NULL,
                            [UserId] INT NOT NULL FOREIGN KEY REFERENCES Users(UserId)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAddress_Insert]') AND type in (N'P'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[UserAddress_Insert]
                            @Address NVARCHAR(255),
                            @City NVARCHAR(100),
                            @State NVARCHAR(100),
                            @Type NVARCHAR(50),
                            @UserId INT
                            AS
                            BEGIN
                                INSERT INTO UserAddresses (Address, City, State, Type, UserId)
                                VALUES (@Address, @City, @State, @Type, @UserId)
                                SELECT CAST(SCOPE_IDENTITY() as int)
                            END
                        ')
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAddress_SelectAll]') AND type in (N'P'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[UserAddress_SelectAll]
                            AS
                            BEGIN
                                SELECT * FROM UserAddresses
                            END
                        ')
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAddress_SelectById]') AND type in (N'P'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[UserAddress_SelectById]
                            @AddressId INT
                            AS
                            BEGIN
                                SELECT * FROM UserAddresses WHERE AddressId = @AddressId
                            END
                        ')
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAddress_Update]') AND type in (N'P'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[UserAddress_Update]
                            @AddressId INT,
                            @Address NVARCHAR(255),
                            @City NVARCHAR(100),
                            @State NVARCHAR(100),
                            @Type NVARCHAR(50),
                            @UserId INT
                            AS
                            BEGIN
                                UPDATE UserAddresses
                                SET Address = @Address,
                                    City = @City,
                                    State = @State,
                                    Type = @Type,
                                    UserId = @UserId
                                WHERE AddressId = @AddressId
                            END
                        ')
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAddress_Delete]') AND type in (N'P'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[UserAddress_Delete]
                            @AddressId INT
                            AS
                            BEGIN
                                DELETE FROM UserAddresses WHERE AddressId = @AddressId
                            END
                        ')
                    END
                ");
            }
        }
    }
}
