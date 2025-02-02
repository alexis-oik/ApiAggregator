namespace ApiAggregator.Features.Statistic
{
    public static class StatisticEndpoint
    {
        public static void MapStatisticEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/statistics", (StatisticService statisticService) =>
            {
                var statistics = statisticService.GetStatistics();

                if (statistics.Count == 0)
                {
                    return Results.StatusCode(204);
                }

                return Results.Ok(statisticService.GetStatistics());
            })
                .Produces<Dictionary<string, Statistics>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Statistics");
        }
    }
}
