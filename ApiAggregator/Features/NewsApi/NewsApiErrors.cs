using ApiAggregator.Errors;

namespace ApiAggregator.Features.NewsApi
{
    public static class NewsApiErrors
    {
        public static Error FailedResponseError => new(
            "NewsApi.FailedResponseError",
            "Failed to get news. See logs for details.");

        public static Error EmptyResponseError => new(
            "NewsApi.EmptyResponseError",
            "Failed to get news. Empty response");

        public static Error DeserializationError => new(
            "NewsApi.DeserializationError",
            "Failed to deserialize news response");
    }
}