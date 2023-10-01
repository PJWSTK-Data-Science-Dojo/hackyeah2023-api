using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

{
    [TestFixture]
    public class SqlQueryExecutorTests
    {
        private SqlQueryExecutor _sqlQueryExecutor;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _configuration = Options.Create(config).Value;
            _sqlQueryExecutor = new SqlQueryExecutor(_configuration);
        }

        [Test]
        public async Task ExecuteQueryAsync_WithValidQuery_ShouldReturnData()
        {
            // Arrange
            var validSqlQuery = "SELECT * FROM TableName"; // Zastąp 'TableName' prawidłową nazwą tabeli
            
            // Act
            var (result, errorMessage) = await _sqlQueryExecutor.ExecuteQueryAsync(validSqlQuery);

            // Assert
            Assert.IsNull(errorMessage);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task ExecuteQueryAsync_WithInvalidQuery_ShouldReturnErrorMessage()
        {
            // Arrange
            var invalidSqlQuery = "SELECT * FROM NonExistentTable"; // Zastąp 'NonExistentTable' nieistniejącą tabelą
            
            // Act
            var (result, errorMessage) = await _sqlQueryExecutor.ExecuteQueryAsync(invalidSqlQuery);

            // Assert
            Assert.IsNotNull(errorMessage);
            Assert.IsNull(result);
           
        }

    }
}
