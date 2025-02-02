﻿using Newtonsoft.Json;

namespace ApiAggregator.Features.Spotify
{
    public class SpotifyTokenResponse
    {

        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

}
