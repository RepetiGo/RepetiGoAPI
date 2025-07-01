namespace RepetiGo.Api.Dtos.StatsDtos
{
    public class ActivityStatsResponse
    {
        public List<MapPointData> DailyReviewCounts { get; set; } = new List<MapPointData>();
        public double DailyAverage { get; set; }
        public int DayLearnedPercent { get; set; }
        public int LongestStreak { get; set; }
        public int CurrentStreak { get; set; }
    }
}
