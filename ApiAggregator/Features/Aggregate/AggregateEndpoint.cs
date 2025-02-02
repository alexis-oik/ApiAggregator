using ApiAggregator.Features.NewsApi;
using ApiAggregator.Features.OpenWeatherMap;
using ApiAggregator.Features.Spotify;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace ApiAggregator.Features.Aggregate
{
    public static class AggregateEndpoint
    {
        public static void MapAggregateEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/aggregate", async (string? sortBy, string? filterBy, IOpenWeatherMapService openWeatherMapService, INewsApiService newsApiService, ISpotifyService spotifyService) =>
            {
                var weatherTask = openWeatherMapService.GetWeatherForecastAsync("Athens,GR");

                var newsTask = newsApiService.GetNewsAsync("AI");

                var spotifyTask = spotifyService.GetSpotifyPlaylistAsync();

                var weather = await weatherTask;

                var news = await newsTask;

                var spotify = await spotifyTask;

                if (news.IsFailure && weather.IsFailure && spotify.IsFailure)
                {
                    return Results.StatusCode(503);
                }

                if (news.Data is not null)
                {
                    news.Data.Articles = NewsApiService.FilterAndSortArticles(news.Data.Articles!, sortBy, filterBy);
                }

                return Results.Ok(news.Data);
            })
                .Produces<AggregateResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status503ServiceUnavailable)
                .WithName("Feed");
        }
    }
}
