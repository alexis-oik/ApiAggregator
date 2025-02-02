using ApiAggregator.Shared;

namespace ApiAggregator.Features.OpenWeatherMap
{
    public interface IOpenWeatherMapService
    {
        Task<Result<OpenWeatherMapResponse>> GetWeatherForecastAsync(string city);
    }
}
