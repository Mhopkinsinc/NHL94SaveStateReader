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
            public string Player { get; set; }            
            public string Team { get; set; }
            //[System.ComponentModel.DefaultValue(0)]
            public int Goals { get; set; }
            //[System.ComponentModel.DefaultValue(0)]
            public int Assisst { get; set; }
            public int Points { get { return (this.Goals + this.Assisst); } }
            public int PenaltyMinutes { get; set; }
        }
    }
}
