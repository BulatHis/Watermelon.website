using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Watermelon.website.Models;

namespace Watermelon.website;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext()
    {
        _connectionString = $"Server=(LocalDB)\\MSSQLLocalDB;Database=WatermelonBase;Encrypt=False;Integrated " +
                            $"Security=True;TrustServerCertificate=True;";
    }

    public async Task<IEnumerable<PriceModel>> ShowPrice(string watermelonName)
    {
        var sqlExpression = $@"SELECT price from Watermelons WHERE Id='{watermelonName}'";
        await using var db = new SqlConnection(_connectionString);
        return await db.QueryAsync<PriceModel>(sqlExpression);
    }

    public async Task<IEnumerable<SaleModel>> ShowSale(string watermelonName)
    {
        var sqlExpression = $@"SELECT salePrice from Watermelons WHERE name='{watermelonName}'";
        await using var db = new SqlConnection(_connectionString);
        return await db.QueryAsync<SaleModel>(sqlExpression);
    }

    public async Task<List<ShowComments>> GetCommentByPage(string watermelonName)
    {
        var sqlExpression = $@"SELECT Email,Comment FROM Comments WHERE watermelonName = '{watermelonName}'";
        await using var db = new SqlConnection(_connectionString);
        return (await db.QueryAsync<ShowComments>(sqlExpression)).ToList();
    }

    public async Task<Resume> AddResume(Resume resume)
    {
        var id = Guid.NewGuid();
        var sqlExpression =
            $@"INSERT INTO GetJob (Id,Phone,Email,Education,Experience,City) VALUES
                                                       ('{id}',
                                                        '{resume.Phone}',
                                                        '{resume.Email}',
                                                        N'{resume.Education}',
                                                        N'{resume.Experience}',
                                                        N'{resume.City}')";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sqlExpression, connection);
        var result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
        {
            return null;
        }

        resume.Id = id;
        return resume;
    }

    public async Task<Resume> GetResumeByEmail(string email)
    {
        var sqlExpression = $@"SELECT email FROM GetJob WHERE Email = '{email}'";
        using IDbConnection db = new SqlConnection(_connectionString);
        return await db.QuerySingleOrDefaultAsync<Resume>(sqlExpression);
    }

    public async Task<Resume> DeleteResume(string email)
    {
        var sqlExpression = $@"DELETE FROM GetJob WHERE Email = '{email}'";
        using IDbConnection db = new SqlConnection(_connectionString);
        return await db.QuerySingleOrDefaultAsync<Resume>(sqlExpression);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var sqlExpression = $@"SELECT Email FROM Users WHERE Email = '{email}'";
        using IDbConnection db = new SqlConnection(_connectionString);
        return await db.QuerySingleOrDefaultAsync<User>(sqlExpression);
    }

    public async Task<BuyModel> AddBuyRequestWhitSale(BuyModel buy)
    {
        var id = Guid.NewGuid();
        var sqlExpression =
            $@"INSERT INTO BuyRequests (Id, watermelonName, address, countOfWatermelon,email,sale,amount) VALUES 
            (
             '{id}',
             '{buy.WatermelonName}'
             ,N'{buy.Address}'
             ,'{buy.CountOfWatermelon}'
             ,'{buy.Email}'
             ,'{buy.Sale}'
             ,(SELECT w.salePrice*'{buy.CountOfWatermelon}' from Watermelons w 
                 where w.name = '{buy.WatermelonName}'))";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sqlExpression, connection);
        var result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
        {
            return null;
        }
        buy.Id = id;
        return buy;
    }
    
    public async Task<BuyModel> AddBuyRequestNoSale(BuyModel buy)
    {
        var id = Guid.NewGuid();
        var sqlExpression =
            $@"INSERT INTO BuyRequests (Id, watermelonName, address, countOfWatermelon,email,sale,amount) VALUES 
            (
             '{id}',
             '{buy.WatermelonName}'
             ,N'{buy.Address}'
             ,'{buy.CountOfWatermelon}'
             ,'{buy.Email}'
             ,'{buy.Sale}'
             ,(SELECT w.price*'{buy.CountOfWatermelon}' from Watermelons w 
                 where w.name = '{buy.WatermelonName}'))";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sqlExpression, connection);
        var result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
        {
            return null;
        }
        buy.Id = id;
        return buy;
    }

    public async Task<Comments> AddComment(Comments comments)
    {
        var id = Guid.NewGuid();
        var sqlExpression =
            $@"INSERT INTO Comments (Id,WatermelonName,Email,Comment) VALUES
                                                       ('{id}',
                                                        '{comments.WatermelonName}',
                                                        '{comments.Email}',
                                                        N'{comments.Comment}')";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sqlExpression, connection);
        var result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
        {
            return null;
        }

        comments.Id = id;
        return comments;
    }

    public async Task<User> AddUser(User user)
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        var id = Guid.NewGuid();
        var sqlExpression =
            $@"INSERT INTO Users (Id, Email, Mobile, Password) VALUES 
            (
             '{id}',
             '{user.Email}'
             ,'{user.Mobile}'
             ,'{user.Password}')";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sqlExpression, connection);
        var result = await command.ExecuteNonQueryAsync();
        if (result <= 0)
        {
            return null;
        }

        user.Id = id;
        return user;
    }


    public async Task<User> GetUser(LoginModel loginModel)
    {
        var sqlExpression = $@"select Password,Email,Password from Users where Email = '{loginModel.Email}'";
        using IDbConnection db = new SqlConnection(_connectionString);
        var user = db.QuerySingleOrDefault<User>(sqlExpression);
        return user != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password)
            ? user
            : null;
    }
}