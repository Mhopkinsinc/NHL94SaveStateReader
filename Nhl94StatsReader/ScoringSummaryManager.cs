using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class ScoringSummaryManager : IDisposable
    {

        //todo Add IDispose Pattern to this class for cleanup.
        
        #region Properties

        IStatReader _statreader;
        List<IStat> _Stats;
        // Flag: Has Dispose already been called? 
        bool disposed = false;

        #endregion

        #region Constructors

        public ScoringSummaryManager(IStatReader Statreader, List<IStat> Stats)
        {
            _statreader = Statreader;
            _Stats = Stats;
            //GetScoringSummary();
        }

        ~ScoringSummaryManager()
        {            
            Dispose();            
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
                var TypeOfGoal = GetGoalType(CurrentScoringOffset + 2);                
                var TeamThatScoredGoal = GetTeamAbbrv(HomeorAwayTeam);
                var GoalScorer = GetGoalScorer(CurrentScoringOffset + 3,HomeorAwayTeam);
                var Assist1 = GetGoalScorer(CurrentScoringOffset + 4, HomeorAwayTeam);
                var Assist2 = GetGoalScorer(CurrentScoringOffset + 5, HomeorAwayTeam);
                var ss = new ScoringSummaryModel.ScoringSummary() { Time = TimeOfGoal, Period = PeriodOfGoal, Assist1 = Assist1, Assist2 = Assist2, Goal = GoalScorer, GoalType = TypeOfGoal, Team = TeamThatScoredGoal };
                SSM.Add(ss);

                CurrentScoringOffset += 6;

            }

            return SSM;
            //_boxscore.scoringsummary = SSM;

        }

        internal string GetTeamAbbrv(HomeorAwayTeam HomeorAway)
        {
            //todo There is duplicate code that can be broken out into more generic method

            int TeamId = GetTeamId(HomeorAway);

            var playermodel = JsonConvert.DeserializeObject<Classic94PlayerModel>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json")));

            var getteams = playermodel.Select(x => x.Team).Distinct().ToList();

            var getteamabbrv = getteams[TeamId];

            return getteamabbrv;
        }

        /// <summary>
        /// Returns The TeamId Of The Home Or Away Team
        /// </summary>
        /// <param name="HomeorAway"></param>
        /// <returns>TeamId</returns>
        private int GetTeamId(HomeorAwayTeam HomeorAway)
        {
            int TeamId;

            // Get All IntegerStats from _Stats
            var IntStats = from p in _Stats
                           where p.GetType() == typeof(IntegerStat)
                           select p;

            // This Will Return The ID of The Home Or Away Team
            switch (HomeorAway)
            {
                case HomeorAwayTeam.Home:
                    TeamId = (from IntegerStat p in IntStats
                              where p.Statname == "Home Team ID"
                              select p._statValueInt).FirstOrDefault();
                    break;
                case HomeorAwayTeam.Away:
                    TeamId = (from IntegerStat p in IntStats
                              where p.Statname == "Away Team ID"
                              select p._statValueInt).FirstOrDefault();
                    break;
                default:
                    TeamId = 0;
                    break;
            }

            return TeamId;
        }

        internal string GetGoalScorer(long Offset, HomeorAwayTeam HomeorAway)
        {
            //todo There is duplicate code that can be broken out into more generic method

            var PlayerId = _statreader.ReadStat(Offset);

            if (PlayerId == 255) return string.Empty;

            int TeamId = GetTeamId(HomeorAway);

            var playermodel = JsonConvert.DeserializeObject<Classic94PlayerModel>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json")));

            var getteams = playermodel.Select(x => x.Team).Distinct().ToList();

            var getteamabbrv = getteams[TeamId];

            var getplayer = (from p in playermodel where p.Team == getteamabbrv && p.RosterID == PlayerId + 1 select new { Name = p.FirstName + ", " + p.LastName }).FirstOrDefault();

            return getplayer.ToString();

        }

        internal HomeorAwayTeam GetHomeorAwayTeam(long Offset)
        {
            int result = _statreader.ReadStat(Offset);
            var goaltype = new GoalType();
            HomeorAwayTeam teamtype;

            teamtype = (result > 4) ? HomeorAwayTeam.Away : HomeorAwayTeam.Home;

            return teamtype;
        }

        internal GoalType GetGoalType(long Offset)
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

        internal int GetPeriod(long Offset)
        {
            var result = _statreader.ReadStat(Offset);
            var period = result / 64 + 1;

            //LOG
            Console.WriteLine("Period: " + period);

            return period;
        }

        internal string GetTime(long Offset)
        {

            var result = _statreader.ReadStat(Offset);
            var timespan = TimeSpan.FromSeconds(result);

            //LOG            
            Console.WriteLine(timespan);

            return timespan.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _statreader = null;
            }

            Console.WriteLine("Disposing Scoring Summary Manager");

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        #endregion
    }
}
