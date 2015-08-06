using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class PlayerStatsModel : List<PlayerStatsModel.PlayerStats>
    {
        public class PlayerStats
        {
            public string Time { get; set; }
            public int Period { get; set; }
            public string Team { get; set; }
            public string Goal { get; set; }
            public string Assist1 { get; set; }
            public string Assist2 { get; set; }
            public GoalType GoalType { get; set; }
        }
    }
}
