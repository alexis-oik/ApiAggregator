using ApiAggregator.Shared;

namespace ApiAggregator.Features.Spotify
{
    public interface ISpotifyService
    {
        Task<Result<string>> GetSpotifyTokenAsync();
        Task<Result<SpotifyPlaylistResponse>> GetSpotifyPlaylistAsync();
    }
}
