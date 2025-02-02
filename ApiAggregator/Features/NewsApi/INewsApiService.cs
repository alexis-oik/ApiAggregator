using ApiAggregator.Shared;

namespace ApiAggregator.Features.NewsApi
{
    public interface INewsApiService
    {
        Task<Result<NewsApiResponse>> GetNewsAsync(string query);
    }
}
