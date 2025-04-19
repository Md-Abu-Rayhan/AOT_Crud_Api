using AOTCrudApi.Models;
using System.Data.SqlClient;

namespace AOTCrudApi.Services
{
    public class ProductService
    {
        private readonly DatabaseService _dbService;

        public ProductService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            using var connection = _dbService.CreateConnection();
            var sqlConnection = (SqlConnection)connection;

            using var command = new SqlCommand("SELECT Id, Name, Description, Price, Stock FROM Products", sqlConnection);
            await sqlConnection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Stock = reader.GetInt32(4)
                });
            }
            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var connection = _dbService.CreateConnection();
            var sqlConnection = (SqlConnection)connection;

            using var command = new SqlCommand("SELECT Id, Name, Description, Price, Stock FROM Products WHERE Id = @Id", sqlConnection);
            command.Parameters.AddWithValue("@Id", id);

            await sqlConnection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Stock = reader.GetInt32(4)
                };
            }

            return null;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            using var connection = _dbService.CreateConnection();
            var sqlConnection = (SqlConnection)connection;

            using var command = new SqlCommand(@"
                INSERT INTO Products (Name, Description, Price, Stock)
                VALUES (@Name, @Description, @Price, @Stock);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", sqlConnection);

            command.Parameters.AddWithValue("@Name", product.Name ?? string.Empty);
            command.Parameters.AddWithValue("@Description", product.Description ?? string.Empty);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);

            await sqlConnection.OpenAsync();
            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            using var connection = _dbService.CreateConnection();
            var sqlConnection = (SqlConnection)connection;

            using var command = new SqlCommand(@"
                UPDATE Products
                SET Name = @Name, Description = @Description, Price = @Price, Stock = @Stock
                WHERE Id = @Id;", sqlConnection);

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name ?? string.Empty);
            command.Parameters.AddWithValue("@Description", product.Description ?? string.Empty);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);

            await sqlConnection.OpenAsync();
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using var connection = _dbService.CreateConnection();
            var sqlConnection = (SqlConnection)connection;

            using var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", sqlConnection);
            command.Parameters.AddWithValue("@Id", id);

            await sqlConnection.OpenAsync();
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
    }
}
