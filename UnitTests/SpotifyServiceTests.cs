using ApiAggregator.Features.Spotify;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregator.UnitTests
{
    public class SpotifyServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<SpotifyService>> _loggerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly ISpotifyService _spotifyService;

        public SpotifyServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<SpotifyService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _configurationMock.Setup(c => c["Spotify:TokenUrl"]).Returns("https://test.token.url");
            _configurationMock.Setup(c => c["Spotify:ClientId"]).Returns("test_client_id");
            _configurationMock.Setup(c => c["Spotify:ClientSecret"]).Returns("test_client_secret");
            _configurationMock.Setup(c => c["Spotify:ApiUrl"]).Returns("https://api.spotify.com/v1/playlists/");

            _spotifyService = new SpotifyService(_httpClient, _configurationMock.Object, _loggerMock.Object, _memoryCache);
        }

        [Fact]
        public async Task GetSpotifyTokenAsync_ReturnsSuccess_WhenTokenIsRetrieved()
        {
            // Arrange
            var expectedToken = "test_token";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"access_token\":\"{expectedToken}\",\"token_type\":\"bearer\",\"expires_in\":\"3600\"}}")
                });

            // Act
            var result = await _spotifyService.GetSpotifyTokenAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().Be(expectedToken);
        }

        [Fact]
        public async Task GetSpotifyTokenAsync_ReturnsFailure_WhenApiCallFails()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            // Act
            var result = await _spotifyService.GetSpotifyTokenAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.FailedResponseError.Code);
        }

        [Fact]
        public async Task GetSpotifyTokenAsync_ReturnsFailure_WhenResponseIsEmpty()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            // Act
            var result = await _spotifyService.GetSpotifyTokenAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.EmptyResponseError.Code);
        }

        [Fact]
        public async Task GetSpotifyTokenAsync_ReturnsFailure_WhenDeserializationFails()
        {
            // Arrange
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
            var result = await _spotifyService.GetSpotifyTokenAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.DeserializationError.Code);
        }

        [Fact]
        public async Task GetSpotifyPlaylistAsync_ReturnsSuccess_WhenPlaylistIsRetrieved()
        {
            // Arrange
            var expectedToken = "test_token";
            var expectedResponse = new SpotifyPlaylistResponse();
            var responseJson = JsonConvert.SerializeObject(expectedResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"access_token\":\"{expectedToken}\",\"token_type\":\"bearer\",\"expires_in\":\"3600\"}}")
                });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => 
                        x.RequestUri!.ToString().Contains("api.spotify.com/v1/playlists/")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            // Act
            var result = await _spotifyService.GetSpotifyPlaylistAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetSpotifyPlaylistAsync_ReturnsFailure_WhenTokenRequestFails()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            // Act
            var result = await _spotifyService.GetSpotifyPlaylistAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.FailedResponseError.Code);
        }

        [Fact]
        public async Task GetSpotifyPlaylistAsync_ReturnsFailure_WhenPlaylistRequestFails()
        {
            // Arrange
            var expectedToken = "test_token";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"access_token\":\"{expectedToken}\",\"token_type\":\"bearer\",\"expires_in\":\"3600\"}}")
                });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x =>
                        x.RequestUri!.ToString().Contains("api.spotify.com/v1/playlists/")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            // Act
            var result = await _spotifyService.GetSpotifyPlaylistAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.FailedResponseError.Code);
        }

        [Fact]
        public async Task GetSpotifyPlaylistAsync_ReturnsFailure_WhenPlaylistResponseIsEmpty()
        {
            // Arrange
            var expectedToken = "test_token";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"access_token\":\"{expectedToken}\",\"token_type\":\"bearer\",\"expires_in\":\"3600\"}}")
                });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => 
                        x.RequestUri!.ToString().Contains("api.spotify.com/v1/playlists/")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            // Act
            var result = await _spotifyService.GetSpotifyPlaylistAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.EmptyResponseError.Code);
        }

        [Fact]
        public async Task GetSpotifyPlaylistAsync_ReturnsFailure_WhenPlaylistDeserializationFails()
        {
            // Arrange
            var expectedToken = "test_token";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"access_token\":\"{expectedToken}\",\"token_type\":\"bearer\",\"expires_in\":\"3600\"}}")
                });

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => 
                        x.RequestUri!.ToString().Contains("api.spotify.com/v1/playlists/")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Invalid JSON")
                });

            // Act
            var result = await _spotifyService.GetSpotifyPlaylistAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be(SpotifyErrors.DeserializationError.Code);
        }
    }
}
