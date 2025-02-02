using ApiAggregator.Errors;

namespace ApiAggregator.Features.Spotify
{
    public static class SpotifyErrors
    {
        public static Error FailedResponseError => new(
            "Spotify.FailedResponseError",
            "Failed to get Spotify playlist. See logs for details.");

        public static Error EmptyResponseError => new(
            "Spotify.EmptyResponseError",
            "Failed to get Spotify playlist. Empty response");

        public static Error DeserializationError => new(
            "Spotify.DeserializationError",
            "Failed to deserialize Spotify playlist response");
    }
}
