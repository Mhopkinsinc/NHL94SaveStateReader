using System;
using NLog;


namespace Nhl94StatsReader
{
    public class ScoringSummaryManager : IDisposable
    {
        
        #region Properties

        IStatReader _statreader;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        public ScoringSummaryManager(IStatReader Statreader)
        {
            if (Statreader == null) { logger.Error("Statreader was null"); throw new ArgumentNullException("Statreader Can't be null."); }
            _statreader = Statreader;
        }              

        #endregion

        #region Methods

        
        public ScoringSummaryModel GetScoringSummary()
        {

            var GoalsScored = _statreader.ReadStat(15693) / 6;

            ScoringSummaryModel SSM = new ScoringSummaryModel();

            long StartingScoringOffset = 15695;
            long CurrentScoringOffset = StartingScoringOffset;


            for (int i = 1; i <= GoalsScored; i++)
            {
                var TimeOfGoal = GetTime(CurrentScoringOffset);
                var PeriodOfGoal = GetPeriod(CurrentScoringOffset + 1);
                var HomeorAwayTeam = GetHomeorAwayTeam(CurrentScoringOffset + 2);
                var TeamId = GetTeamID(HomeorAwayTeam);
                var TeamAbbreviation = Utils.GetTeamAbbreviation(TeamId);
                var TypeOfGoal = GetGoalType(CurrentScoringOffset + 2);
                var GoalScorer = GetPlayer(CurrentScoringOffset + 3,TeamAbbreviation );
                var Assist1 = GetPlayer(CurrentScoringOffset + 4, TeamAbbreviation);
                var Assist2 = GetPlayer(CurrentScoringOffset + 5, TeamAbbreviation);
                var ss = new ScoringSummaryModel.ScoringSummary() { Time = TimeOfGoal, Period = PeriodOfGoal, Assist1 = Assist1, Assist2 = Assist2, Goal = GoalScorer, GoalType = TypeOfGoal, Team = TeamAbbreviation };
                SSM.Add(ss);

                CurrentScoringOffset += 6; //Move To The Next Scoring Summary Offset

            }

            return SSM;
        }

        private int GetTeamID(HomeorAwayTeam HomeorAway)
        {
            int teamid;

            teamid = (HomeorAway == HomeorAwayTeam.Home) ? _statreader.ReadStat(10411) : _statreader.ReadStat(10413);

            return teamid;

        }       
      
        private string GetPlayer(long offset, string TeamAbbreviation)
        {
            var rosterid = _statreader.ReadStat(offset);

            if (rosterid == 255) return string.Empty;

            var player = Utils.GetPlayer(TeamAbbreviation,rosterid);

            return player;
        }       

        private HomeorAwayTeam GetHomeorAwayTeam(long Offset)
        {
            int result = _statreader.ReadStat(Offset);            
            HomeorAwayTeam teamtype;

            teamtype = (result > 4) ? HomeorAwayTeam.Away : HomeorAwayTeam.Home;

            return teamtype;
        }

        private GoalType GetGoalType(long Offset)
        {
            int result = _statreader.ReadStat(Offset);
            var goaltype = new GoalType();            
           
            result = (result > 4) ? (result - 128) : result;

            switch (result)
            {
                case 0:
                    goaltype = GoalType.ShortHanded2;                    
                    break;
                case 1:
                    goaltype = GoalType.ShortHanded;                    
                    break;
                case 2:
                    goaltype = GoalType.EvenStrength;                    
                    break;
                case 3:
                    goaltype = GoalType.PowerPlay;                    
                    break;
                case 4:
                    goaltype = GoalType.PowerPlay2;                    
                    break;                

                default:
                    //ERROR                   
                    break;
            }

            return goaltype;

        }

        private int GetPeriod(long Offset)
        {
            var result = _statreader.ReadStat(Offset);
            var period = result / 64 + 1;

            return period;
        }

        private string GetTime(long Offset)
        {

            var result = _statreader.ReadStat(Offset);
            var timespan = TimeSpan.FromSeconds(result);

            return timespan.ToString(@"mm\:ss");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _statreader = null;
                }

                disposedValue = true;
            }
        }

        ~ScoringSummaryManager()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
