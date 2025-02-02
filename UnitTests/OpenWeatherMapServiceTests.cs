using ApiAggregator.Features.OpenWeatherMap;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace ApiAggregator.UnitTests
{
    public class OpenWeatherMapServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<OpenWeatherMapService>> _loggerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly IOpenWeatherMapService _openWeatherMapService;

        public OpenWeatherMapServiceTests() 
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<OpenWeatherMapService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _configurationMock.Setup(c => c["OpenWeatherMap:ApiUrl"]).Returns("https://api.openweathermap.org/data/2.5");
            _configurationMock.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns("test_api_key");

            _openWeatherMapService = new OpenWeatherMapService(_httpClient, _configurationMock.Object, _loggerMock.Object, _memoryCache);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsSuccess_WhenApiCallIsSuccessful()
        {
            // Arrange
            var city = "Athens";
            var expectedResponse = new OpenWeatherMapResponse();
            var jsonResponse = JsonConvert.SerializeObject(expectedResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _openWeatherMapService.GetWeatherForecastAsync(city);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsFailure_WhenApiCallFails()
        {
            // Arrange
            var city = "Athens";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                });

            // Act
            var result = await _openWeatherMapService.GetWeatherForecastAsync(city);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(OpenWeatherMapErrors.FailedResponseError.Code);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsFailure_WhenResponseIsEmpty()
        {
            // Arrange
            var city = "Athens";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty),
                });

            // Act
            var result = await _openWeatherMapService.GetWeatherForecastAsync(city);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(OpenWeatherMapErrors.EmptyResponseError.Code);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsFailure_WhenDeserializationFails()
        {
            // Arrange
            var city = "Athens";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Invalid JSON")
                });

            // Act
            var result = await _openWeatherMapService.GetWeatherForecastAsync(city);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(OpenWeatherMapErrors.DeserializationError.Code);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ReturnsCachedWeather_WhenCacheHit()
        {
            // Arrange
            var city = "Athens";
            var cachedResponse = new OpenWeatherMapResponse();
            _memoryCache.Set("Weather", cachedResponse, TimeSpan.FromHours(24));

            var service = new OpenWeatherMapService(_httpClient, _configurationMock.Object, _loggerMock.Object, _memoryCache);

            // Act
            var result = await service.GetWeatherForecastAsync(city);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(cachedResponse);
        }
    }
}
