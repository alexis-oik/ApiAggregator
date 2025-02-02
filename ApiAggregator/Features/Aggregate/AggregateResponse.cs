using ApiAggregator.Features.NewsApi;
using ApiAggregator.Features.OpenWeatherMap;
using ApiAggregator.Features.Spotify;

namespace ApiAggregator
{
    public record AggregateResponse(OpenWeatherMapResponse? Weather, NewsApiResponse? News, SpotifyPlaylistResponse? Spotify);
}
