using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class PenaltySummaryModel : List<PenaltySummaryModel.PenaltySummary>
    {
        public class PenaltySummary
        {
            public string Time { get; set; }
            public int Period { get; set; }
            public string Team { get; set; }
            public string Player { get; set; }                  
            public PenaltyType PenaltyType { get; set; }
        }
    }
}
