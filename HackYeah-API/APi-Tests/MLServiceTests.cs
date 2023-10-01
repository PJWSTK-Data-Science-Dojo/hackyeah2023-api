using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using HackYeah_API.Services.Interfaces;
using HackYeah_API.Models;

{
    [TestFixture]
    public class MLServiceTests
    {
        private MLService _mlService;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mlService = new MLService(
                _mockConfiguration.Object,
                _mockHttpClientFactory.Object
            );
        }

        [Test]
        public async Task SendDDL_WithValidPayload_ShouldReturnSuccess()
        {
            // Arrange
            var validDdl = "Valid DDL";
            var expectedEndpoint = "/chat";
            var expectedApiUrl = "https://example.com/api"; // Ustaw oczekiwany URL API
            var expectedResponseContent = new StringContent("Success", Encoding.UTF8, "application/json");
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockConfiguration.Setup(/* ... */); // Skonfiguruj oczekiwany URL API

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = expectedResponseContent
                });

            _mockHttpClientFactory.Setup(_ => _.CreateClient()).Returns(httpClient);

            // Act
            await _mlService.SendDDL(validDdl);

            // Assert
            _mockHttpClientFactory.Verify(_ => _.CreateClient(), Times.Once);
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == $"{expectedApiUrl}{expectedEndpoint}"),
                ItExpr.IsAny<CancellationToken>()
            );

        }

        [Test]
        public async Task RequestForSQLPrompt_WithValidPrompt_ShouldReturnResponse()
        {
            // Arrange
            var validPrompt = "Valid natural language prompt";
            var expectedEndpoint = "/context";
            var expectedApiUrl = "https://example.com/api"; // Ustaw oczekiwany URL API
            var expectedResponseContent = new MLApiResponse { Output = "Response" };
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockConfiguration.Setup(/* ... */); // Skonfiguruj oczekiwany URL API

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedResponseContent)
                });

            _mockHttpClientFactory.Setup(_ => _.CreateClient()).Returns(httpClient);

            // Act
            var result = await _mlService.RequestForSQLPrompt(validPrompt);

            // Assert
            _mockHttpClientFactory.Verify(_ => _.CreateClient(), Times.Once);
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == $"{expectedApiUrl}{expectedEndpoint}"),
                ItExpr.IsAny<CancellationToken>()
            );

            Assert.AreEqual("Response", result);
        }

    }
}
