using HackYeah_API.Controllers;
using HackYeah_API.Models;
using HackYeah_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace HackYeah.Tests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private AdminController _adminController;
        private IDdlExtractionService _ddlExtractionService;
        private IMLService _mlService;

        [SetUp]
        public void Setup()
        {
            _ddlExtractionService = Substitute.For<IDdlExtractionService>();
            _mlService = Substitute.For<IMLService>();
            _adminController = new AdminController(_ddlExtractionService, _mlService);
        }

        [Test]
        public async Task UploadDatabase_ValidFile_ShouldReturnOkResult()
        {
            // Arrange
            var fileUpload = new FileUploadModel { File = Substitute.For<IFormFile>() };

            // Act
            var result = await _adminController.UploadDatabase(fileUpload) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        // ... other tests
    }
}