using ApiAggregator.Features.Statistic;
using System.Diagnostics;

namespace ApiAggregator.Middleware
{
    public class TimingHttpHandler : DelegatingHandler
    {
        private readonly StatisticService _statisticService;

        public TimingHttpHandler(StatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            var apiName = request.Headers.GetValues("X-Api-Name").First();

            _statisticService.TrackRequest(apiName, stopwatch.ElapsedMilliseconds);

            return response;
        }
    }
}
