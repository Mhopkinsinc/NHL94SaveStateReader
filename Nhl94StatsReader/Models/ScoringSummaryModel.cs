using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public string GoalType { get; set; }
        }        
    }
}
