using System.Net;
using ApiAggregator.Features.NewsApi;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace ApiAggregator.UnitTests
{
    public class NewsApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<NewsApiService>> _loggerMock;
        private readonly INewsApiService _newsApiService;

        public NewsApiServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<NewsApiService>>();

            _configurationMock.Setup(c => c["NewsApi:ApiUrl"]).Returns("https://newsapi.org/v2");
            _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("test_api_key");

            _newsApiService = new NewsApiService(_httpClient, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetNewsAsync_ReturnsSuccess_WhenApiCallIsSuccessful()
        {
            // Arrange
            var expectedResponse = new NewsApiResponse();
            var jsonResponse = JsonConvert.SerializeObject(expectedResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _newsApiService.GetNewsAsync("test");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetNewsAsync_ReturnsFailure_WhenApiCallFails()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            // Act
            var result = await _newsApiService.GetNewsAsync("test");

            // Assert
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(NewsApiErrors.FailedResponseError.Code);
        }

        [Fact]
        public async Task GetNewsAsync_ReturnsFailure_WhenResponseIsEmpty()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            // Act
            var result = await _newsApiService.GetNewsAsync("test");

            // Assert
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(NewsApiErrors.EmptyResponseError.Code);
        }

        [Fact]
        public async Task GetNewsAsync_ReturnsFailure_WhenDeserializationFails()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Invalid JSON")
                });

            // Act
            var result = await _newsApiService.GetNewsAsync("test");

            // Assert
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(NewsApiErrors.DeserializationError.Code);
        }
    }
}
