using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;

{
    [TestFixture]
    public class DdlExtractionServiceTests
    {
        private IDdlExtractionService _ddlExtractionService;
        private IConfiguration _configuration;
        private IMLService _mlService;
        private SqlQueryExecutor _queryExecutor;
        private SQLQueries _sqlQueries;

        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _configuration = Options.Create(config).Value;
            _mlService = new MockMLService(); // Zastąp 'MockMLService' odpowiednią klasą mockową dla IMLService
            _queryExecutor = new MockSqlQueryExecutor(); // Zastąp 'MockSqlQueryExecutor' odpowiednią klasą mockową dla SqlQueryExecutor
            _sqlQueries = new SQLQueries();

            _ddlExtractionService = new DdlExtractionService(_configuration, _mlService, _queryExecutor, _sqlQueries);
        }

        [Test]
        public async Task ExtractDdl_WithValidSqlLiteFile_ShouldReturnDdlString()
        {
            // Arrange
            var sqlLiteFile = new MockFormFile("valid.sqlite"); // Zastąp 'valid.sqlite' odpowiednią nazwą pliku
            
            // Act
            var result = await _ddlExtractionService.ExtractDdl(sqlLiteFile);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task ExtractDdl_WithInvalidSqlLiteFile_ShouldReturnErrorMessage()
        {
            // Arrange
            var invalidSqlLiteFile = new MockFormFile("invalid.sql"); // Zastąp 'invalid.sql' odpowiednią nazwą nieprawidłowego pliku
            
            // Act
            var result = await _ddlExtractionService.ExtractDdl(invalidSqlLiteFile);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Error")); // Dostosuj to do oczekiwanego komunikatu błędu
        }

    }
}
