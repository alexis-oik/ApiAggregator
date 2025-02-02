namespace ApiAggregator.Features.Statistic
{
    public class Statistics
    {
        public int TotalRequests { get; set; }
        public int FastRequests { get; set; }
        public int AverageRequests { get; set; }
        public int SlowRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public List<double> ResponseTimes { get; set; } = [];
    }
}
