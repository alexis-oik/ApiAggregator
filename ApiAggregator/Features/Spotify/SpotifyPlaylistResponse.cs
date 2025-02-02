using Newtonsoft.Json;

namespace ApiAggregator.Features.Spotify
{
    public class ExternalUrls
    {

        [JsonProperty("spotify")]
        public string? Spotify { get; set; }
    }

    public class Followers
    {

        [JsonProperty("href")]
        public object? Href { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class Image
    {

        [JsonProperty("height")]
        public object? Height { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("width")]
        public object? Width { get; set; }
    }

    public class Owner
    {

        [JsonProperty("display_name")]
        public string? DisplayName { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }
    }

    public class AddedBy
    {

        [JsonProperty("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }
    }

    public class Artist
    {

        [JsonProperty("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }
    }

    public class Album
    {

        [JsonProperty("available_markets")]
        public List<string>? AvailableMarkets { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("album_type")]
        public string? AlbumType { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("images")]
        public List<Image>? Images { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonProperty("release_date_precision")]
        public string? ReleaseDatePrecision { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }

        [JsonProperty("artists")]
        public List<Artist>? Artists { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonProperty("total_tracks")]
        public int TotalTracks { get; set; }
    }

    public class ExternalIds
    {

        [JsonProperty("isrc")]
        public string? Isrc { get; set; }
    }

    public class Track
    {

        [JsonProperty("preview_url")]
        public object? PreviewUrl { get; set; }

        [JsonProperty("available_markets")]
        public IList<string> AvailableMarkets { get; set; }

        [JsonProperty("explicit")]
        public bool Explicit { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("episode")]
        public bool? Episode { get; set; }

        [JsonProperty("track")]
        public bool IsTrack { get; set; }

        [JsonProperty("album")]
        public Album? Album { get; set; }

        [JsonProperty("artists")]
        public List<Artist>? Artists { get; set; }

        [JsonProperty("disc_number")]
        public int DiscNumber { get; set; }

        [JsonProperty("track_number")]
        public int TrackNumber { get; set; }

        [JsonProperty("duration_ms")]
        public int DurationMs { get; set; }

        [JsonProperty("external_ids")]
        public ExternalIds? ExternalIds { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }

        [JsonProperty("is_local")]
        public bool IsLocal { get; set; }
    }

    public class VideoThumbnail
    {

        [JsonProperty("url")]
        public object? Url { get; set; }
    }

    public class Item
    {

        [JsonProperty("added_at")]
        public DateTime AddedAt { get; set; }

        [JsonProperty("added_by")]
        public AddedBy? AddedBy { get; set; }

        [JsonProperty("is_local")]
        public bool IsLocal { get; set; }

        [JsonProperty("primary_color")]
        public object? PrimaryColor { get; set; }

        [JsonProperty("track")]
        public Track? Track { get; set; }

        [JsonProperty("video_thumbnail")]
        public VideoThumbnail? VideoThumbnail { get; set; }
    }

    public class Tracks
    {

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("items")]
        public List<Item>? Items { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("next")]
        public object? Next { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("previous")]
        public object? Previous { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class SpotifyPlaylistResponse
    {

        [JsonProperty("collaborative")]
        public bool Collaborative { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }

        [JsonProperty("followers")]
        public Followers? Followers { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("images")]
        public List<Image>? Images { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("owner")]
        public Owner? Owner { get; set; }

        [JsonProperty("primary_color")]
        public object? PrimaryColor { get; set; }

        [JsonProperty("public")]
        public bool Public { get; set; }

        [JsonProperty("snapshot_id")]
        public string? SnapshotId { get; set; }

        [JsonProperty("tracks")]
        public Tracks? Tracks { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("uri")]
        public string? Uri { get; set; }
    }

}
