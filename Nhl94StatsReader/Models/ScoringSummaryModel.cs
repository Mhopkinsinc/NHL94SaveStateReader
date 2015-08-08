using System.Collections.Generic;


namespace Nhl94StatsReader
{
    public class ScoringSummaryModel : List<ScoringSummaryModel.ScoringSummary>
    {
        public class ScoringSummary
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
