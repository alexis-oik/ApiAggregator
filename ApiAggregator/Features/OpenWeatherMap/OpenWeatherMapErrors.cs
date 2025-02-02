using ApiAggregator.Errors;

namespace ApiAggregator.Features.OpenWeatherMap
{
    public static class OpenWeatherMapErrors
    {
        public static Error FailedResponseError => new(
            "OpenWeatherMap.FailedResponseError",
            "Failed to get weather. See logs for details.");

        public static Error EmptyResponseError => new(
            "OpenWeatherMap.EmptyResponseError",
            "Failed to get weather. Empty response");

        public static Error DeserializationError => new(
            "OpenWeatherMap.DeserializationError",
            "Failed to deserialize weather response");
    }
}
