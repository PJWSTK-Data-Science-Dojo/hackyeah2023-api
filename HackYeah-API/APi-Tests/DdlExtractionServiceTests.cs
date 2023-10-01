using HackYeah_API.Services;
using HackYeah_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Text;
using HackYeah_API.Databases;

namespace HackYeah.Tests
{
    [TestFixture]
    public class DdlExtractionServiceTests
    {
        private DdlExtractionService _ddlExtractionService;
        private IMLService _mlService;
        private SqlQueryExecutor _queryExecutor;
        private IConfiguration _config;
        private SQLQueries _sqlQueries;

        [SetUp]
        public void Setup()
        {
            _mlService = Substitute.For<IMLService>();
            _queryExecutor = Substitute.For<SqlQueryExecutor>(Substitute.For<IConfiguration>());
            _config = Substitute.For<IConfiguration>();
            _sqlQueries = new SQLQueries();
            _ddlExtractionService = new DdlExtractionService(_config, _mlService, _queryExecutor, _sqlQueries);
        }

        [Test]
        public async Task ExtractDdl_ValidFile_ShouldReturnExpectedDdl()
        {
            // Arrange
            var file = Substitute.For<IFormFile>();
            file.FileName.Returns("test.db");
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("dummy content"));
            file.OpenReadStream().Returns(memoryStream);

            // Act
            var result = await _ddlExtractionService.ExtractDdl(file);

            // Assert
            // Assuming ExtractDdl always returns "dummy ddl" for simplicity
            Assert.AreEqual("dummy ddl", result);
        }

        // ... other tests
    }
}