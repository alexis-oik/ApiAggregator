using ApiAggregator.Exceptions;
using ApiAggregator.Features.Statistic;
using ApiAggregator.Middleware;
using ApiAggregator.Features.NewsApi;
using ApiAggregator.Features.OpenWeatherMap;
using ApiAggregator.Features.Spotify;
using ApiAggregator.Features.Aggregate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();
builder.Services.AddOutputCache();

builder.Services.AddSingleton<StatisticService>();

builder.Services.AddTransient<TimingHttpHandler>();

builder.Services.AddSingleton<IOpenWeatherMapService, OpenWeatherMapService>();
builder.Services.AddSingleton<INewsApiService, NewsApiService>();
builder.Services.AddSingleton<ISpotifyService, SpotifyService>();
  
builder.Services.AddHttpClient<IOpenWeatherMapService, OpenWeatherMapService>(openWeatherClient =>
{
    openWeatherClient.DefaultRequestHeaders.Add("Accept", "application/json");
    openWeatherClient.DefaultRequestHeaders.Add("User-Agent", "ApiAggregator/1.0");
    openWeatherClient.DefaultRequestHeaders.Add("X-Api-Name", "OpenWeatherMap");

})
    .AddHttpMessageHandler<TimingHttpHandler>()
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<INewsApiService, NewsApiService>(newsApiClient =>
{
    newsApiClient.DefaultRequestHeaders.Add("Accept", "application/json");
    newsApiClient.DefaultRequestHeaders.Add("User-Agent", "ApiAggregator/1.0");
    newsApiClient.DefaultRequestHeaders.Add("X-Api-Name", "NewsApi");

})
    .AddHttpMessageHandler<TimingHttpHandler>()
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<ISpotifyService, SpotifyService>(spotifyClient =>
{
    spotifyClient.DefaultRequestHeaders.Add("User-Agent", "ApiAggregator/1.0");
    spotifyClient.DefaultRequestHeaders.Add("X-Api-Name", "Spotify");

})
    .AddHttpMessageHandler<TimingHttpHandler>()
    .AddStandardResilienceHandler();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAggregateEndpoints();
app.MapStatisticEndpoints();

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseExceptionHandler();

app.Run();
