using ApiAggregator.Shared;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ApiAggregator.Features.OpenWeatherMap
{
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenWeatherMapService> _logger;
        private readonly IMemoryCache _memoryCache;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public OpenWeatherMapService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherMapService> logger, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<Result<OpenWeatherMapResponse>> GetWeatherForecastAsync(string city)
        {

            if (!_memoryCache.TryGetValue("Weather", out OpenWeatherMapResponse? cachedWeather))
            {
                await _semaphore.WaitAsync(5000);

                try
                {
                    if(_memoryCache.TryGetValue("Weather", out cachedWeather))
                    {
                        return Result<OpenWeatherMapResponse>.Success(cachedWeather!);

                    }

                    var response = await _httpClient.GetAsync(
                    $"{_configuration["OpenWeatherMap:ApiUrl"]}/forecast?q={city}&appid={_configuration["OpenWeatherMap:ApiKey"]}");

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError(
                            "Failed to get weather forecast. Status code: {StatusCode}  Error: {Error}",
                            response.StatusCode,
                            await response.Content.ReadAsStringAsync());

                        return Result<OpenWeatherMapResponse>.Failure(OpenWeatherMapErrors.FailedResponseError);
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _logger.LogError("Failed to get weather forecast. Empty response");

                        return Result<OpenWeatherMapResponse>.Failure(OpenWeatherMapErrors.EmptyResponseError);
                    }

                    try
                    {
                        cachedWeather = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(content);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to get weather forecast. Failed to deserialize response");

                        return Result<OpenWeatherMapResponse>.Failure(OpenWeatherMapErrors.DeserializationError);
                    }

                    _memoryCache.Set("Weather", cachedWeather, TimeSpan.FromHours(24));
                }
                finally
                {
                    _semaphore.Release();
                }
            }
          
            return Result<OpenWeatherMapResponse>.Success(cachedWeather!);
        }
    }
}
