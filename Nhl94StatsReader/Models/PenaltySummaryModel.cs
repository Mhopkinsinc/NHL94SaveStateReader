using System.Collections.Generic;


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
