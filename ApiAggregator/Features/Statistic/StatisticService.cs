namespace ApiAggregator.Features.Statistic
{
    public class StatisticService
    {
        private readonly Dictionary<string, Statistics> _apiStatistics = [];

        public void TrackRequest(string apiName, double responseTime)
        {
            if (!_apiStatistics.ContainsKey(apiName))
            {
                _apiStatistics[apiName] = new Statistics();
            }

            var stats = _apiStatistics[apiName];

            stats.TotalRequests++;

            stats.ResponseTimes.Add(responseTime);

            if (responseTime < 100)
            {
                stats.FastRequests++;
            }
            else if (responseTime >= 100 && responseTime <= 200)
            {
                stats.AverageRequests++;
            }
            else
            {
                stats.SlowRequests++;
            }

            stats.AverageResponseTime = stats.ResponseTimes.Average();
        }

        public Dictionary<string, Statistics> GetStatistics()
        {
            return _apiStatistics;
        }
    }
}
