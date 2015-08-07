
using System.Collections.Generic;


namespace Nhl94StatsReader
{
    public class PlayerStatsModel : List<PlayerStatsModel.PlayerStats>
    {
        public class PlayerStats
        {
            public string Player { get; set; }            
            public string Team { get; set; }
            //[System.ComponentModel.DefaultValue(0)]
            public int Goals { get; set; }
            //[System.ComponentModel.DefaultValue(0)]
            public int Assissts { get; set; }
            public int Points { get; set; }
            public int PenaltyMinutes { get; set; }
            public int Shots { get; set; }
        }
    }
}
