using ApiAggregator.Errors;
using ApiAggregator.Shared;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ApiAggregator.Features.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SpotifyService> _logger;
        private readonly IMemoryCache _memoryCache;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public SpotifyService(HttpClient httpClient, IConfiguration configuration, ILogger<SpotifyService> logger, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<Result<string>> GetSpotifyTokenAsync()
        {
            if (!_memoryCache.TryGetValue("AccessToken", out string? cachedToken))
            {
                await _semaphore.WaitAsync(5000);

                try
                {
                    if(_memoryCache.TryGetValue("AccessToken", out cachedToken))
                    {
                        return Result<string>.Success(cachedToken!);
                    }

                    var response = await _httpClient.PostAsync(
                                  _configuration["Spotify:TokenUrl"],
                                  new FormUrlEncodedContent 
                                  (
                                      new Dictionary<string, string>
                                      {
                                        { "grant_type", "client_credentials" },
                                        { "client_id", _configuration["Spotify:ClientId"]! },
                                        { "client_secret", _configuration["Spotify:ClientSecret"]! }
                                      }
                                  ));

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError(
                            "Failed to get spotify. Status code: {StatusCode}  Error: {Error}",
                            response.StatusCode,
                            await response.Content.ReadAsStringAsync());

                        return Result<string>.Failure(SpotifyErrors.FailedResponseError);
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _logger.LogError("Failed to get spotify. Empty response");

                        return Result<string>.Failure(SpotifyErrors.EmptyResponseError);
                    }

                    SpotifyTokenResponse? tokenResponse;

                    try
                    {
                        tokenResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(content);
                    }
                    catch(JsonException ex)
                    {
                        _logger.LogError(ex,"Failed to get spotify access token. Failed to deserialize response");

                        return Result<string>.Failure(SpotifyErrors.DeserializationError);

                    }

                    cachedToken = tokenResponse?.AccessToken;

                    _memoryCache.Set("AccessToken", cachedToken, TimeSpan.FromSeconds(3000));
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return Result<string>.Success(cachedToken!);
        }

        public async Task<Result<SpotifyPlaylistResponse>> GetSpotifyPlaylistAsync()
        {
            var accessTokenRequest = await GetSpotifyTokenAsync();

            if (accessTokenRequest.IsFailure)
            {
                return Result<SpotifyPlaylistResponse>.Failure(accessTokenRequest.Error!);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenRequest.Data);

            var response = await _httpClient.GetAsync(
                _configuration["Spotify:ApiUrl"] + "/playlists/3cEYpjA9oz9GiPac4AsH4n");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to get spotify. Status code: {StatusCode}  Error: {Error}", response.StatusCode, await response.Content.ReadAsStringAsync());

                return Result<SpotifyPlaylistResponse>.Failure(SpotifyErrors.FailedResponseError);
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogError("Failed to get spotify. Empty response");

                return Result<SpotifyPlaylistResponse>.Failure(SpotifyErrors.EmptyResponseError);
            }

            SpotifyPlaylistResponse? responseContent;

            try
            {
                responseContent = JsonConvert.DeserializeObject<SpotifyPlaylistResponse>(content);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to get spotify. Failed to deserialize response");

                return Result<SpotifyPlaylistResponse>.Failure(SpotifyErrors.DeserializationError);
            }

            return Result<SpotifyPlaylistResponse>.Success(responseContent!);
        }
    }
}
