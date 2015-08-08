
namespace Nhl94StatsReader
{
    public class Boxscore
    {
        public ScoringSummaryModel scoringsummary { get; set; }
        public PenaltySummaryModel penaltysummary { get; set; }
        public PlayerStatsModel playerstats { get; set; }
        public TeamStatsModel teamstats { get; set; }
    }
}
