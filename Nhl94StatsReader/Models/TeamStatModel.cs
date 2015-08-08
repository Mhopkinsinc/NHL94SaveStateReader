using System.Collections.Generic;

namespace Nhl94StatsReader
{
    public class TeamStatsModel  : List<TeamStatsModel.TeamStats>
    {
        public class TeamStats
        {
            public HomeorAwayTeam HomeOrAway { get; set; }
            public int TeamID { get; set; }
            public string Team { get; set; }
            public int Score { get; set; }
            public int Shots { get; set; }
            public int Breakaways { get; set; }
            public int BreakawayGoals { get; set; }
            public int OneTimers { get; set; }
            public int OneTimerGoals { get; set; }
            public int PenaltyShots { get; set; }
            public int PenaltyShotGoals { get; set; }
            public int FaceoffsWon { get; set; }
            public int Bodychecks { get; set; }
            public int Passes { get; set; }
            public int PassesReceived { get; set; }
            public int Powerplays { get; set; }
            public int PowerplayGoals { get; set; }
            public int PowerplayShots { get; set; }
            public int ShorthandedGoals { get; set; }
            public int Penalities { get; set; }
            public int PenaltyTime { get; set; }
            public int Period1Shots { get; set; }
            public int Period2Shots { get; set; }
            public int Period3Shots { get; set; }
            public int OTShots { get; set; }
            public int Period1Goals { get; set; }
            public int Period2Goals { get; set; }
            public int Period3Goals { get; set; }
            public int OTGoals { get; set; }
            public string OffensiveTimeZone { get; set; }
            public string PowerplayTime { get; set; }

        }
    }
}
