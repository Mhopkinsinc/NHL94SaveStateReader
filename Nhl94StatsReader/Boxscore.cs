using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class Boxscore
    {
        public ScoringSummaryModel scoringsummary { get; set; }
        public PenaltySummaryModel penaltysummary { get; set; }
        public PlayerStatsModel playerstats { get; set; }
    }
}
