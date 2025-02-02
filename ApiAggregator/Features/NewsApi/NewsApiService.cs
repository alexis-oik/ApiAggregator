using ApiAggregator.Errors;
using ApiAggregator.Shared;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace ApiAggregator.Features.NewsApi
{
    public class NewsApiService : INewsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NewsApiService> _logger;

        public NewsApiService(HttpClient httpClient, IConfiguration configuration, ILogger<NewsApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<NewsApiResponse>> GetNewsAsync(string query)
        {
            _logger.LogError("Getting news for query: {Query}", DateTime.Now.Date);

            var response = await _httpClient.GetAsync(
                $"{_configuration["NewsApi:ApiUrl"]}/everything?q={query}&from={DateTime.Now.Date.ToString("yyyy-dd-MM")}&apiKey={_configuration["NewsApi:ApiKey"]}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to get news. Status code: {StatusCode}  Error: {Error}",
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());

                return Result<NewsApiResponse>.Failure(NewsApiErrors.FailedResponseError);
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogError("Failed to get news. Empty response");

                return Result<NewsApiResponse>.Failure(NewsApiErrors.EmptyResponseError);
            }

            NewsApiResponse? responseJson;

            try
            {
                responseJson = JsonConvert.DeserializeObject<NewsApiResponse>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize news response");

                return Result<NewsApiResponse>.Failure(NewsApiErrors.DeserializationError);
            }

            return Result<NewsApiResponse>.Success(responseJson!);
        }

        public static List<Article> FilterAndSortArticles(IEnumerable<Article> articles, string? sortBy, string? filterBy)
        {
            if (!string.IsNullOrEmpty(filterBy))
            {
                articles = articles.Where(x => x.Source?.Name?.ToLower() == filterBy.ToLower());
            }

            if (sortBy == "date")
            {The API Aggregator is a .NET-core application designed to aggr
                articles = articles.OrderByDescending(x => x.PublishedAt);
            }
            else if (sortBy == "source")
            {
                articles = articles.OrderBy(x => x.Source?.Name);
            }

            return articles.ToList();

        }
    }
}
