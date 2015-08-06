using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class PenaltySummaryManager : IDisposable
    {

        //todo Add IDispose Pattern to this class for cleanup.

        #region Properties

        IStatReader _statreader;
        List<IStat> _Stats;
        // Flag: Has Dispose already been called? 
        bool disposed = false;

        #endregion

        #region Constructors

        public PenaltySummaryManager(IStatReader Statreader, List<IStat> Stats)
        {
            _statreader = Statreader;
            _Stats = Stats;
            GetPenaltySummary();
        }

        ~PenaltySummaryManager()
        {
            Dispose();
        }

        #endregion

        #region Methods


        public PenaltySummaryModel GetPenaltySummary()
        {

            var TotalPenalities = _statreader.ReadStat(8783);

            PenaltySummaryModel PSM = new PenaltySummaryModel();

            long StartingPenaltyOffset = 8785;
            long CurrentPenaltyOffset = StartingPenaltyOffset;


            for (int i = 1; i <= TotalPenalities; i++)
            {
                var TimeOfPenalty = GetTime(CurrentPenaltyOffset);
                var PeriodOfPenalty = GetPeriod(CurrentPenaltyOffset + 1);
                //var PenaltyPlayerIDAndTeam = HomeorAwayTeamAndPlayer(CurrentPenaltyOffset + 2);
                //var PenaltyTeamAndType = GetGoalType(CurrentPenaltyOffset + 2);
                //var TeamThatScoredGoal = GetTeamAbbrv(GoalTeamAndType.homeorawayteam);
                //var GoalScorer = GetGoalScorer(CurrentPenaltyOffset + 3, GoalTeamAndType.homeorawayteam);
                //var Assist1 = GetGoalScorer(CurrentPenaltyOffset + 4, GoalTeamAndType.homeorawayteam);
                //var Assist2 = GetGoalScorer(CurrentPenaltyOffset + 5, GoalTeamAndType.homeorawayteam);
                //var ss = new ScoringSummaryModel.ScoringSummary() { Time = TimeOfGoal, Period = PeriodOfGoal, Assist1 = Assist1, Assist2 = Assist2, Goal = GoalScorer, GoalType = GoalTeamAndType.typeofgoal, Team = TeamThatScoredGoal };
                //PSM.Add(ss);

                CurrentPenaltyOffset += 4;

            }

            return PSM;
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

        internal HomeorAwayandPlayerId GetPlayerIDandHomeOrAwayTeam(long Offset)
        {
            var result = _statreader.ReadStat(Offset).ToString("X");
            var goaltype = new GoalType();
            HomeorAwayTeam teamtype;

            switch (result)
            {
                case "01":
                    goaltype = GoalType.ShortHanded2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "02":
                    goaltype = GoalType.ShortHanded;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "03":
                    goaltype = GoalType.EvenStrength;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "04":
                    goaltype = GoalType.PowerPlay;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "06":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "07":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "08":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "09":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "0A":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "0B":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "0C":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "0D":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "0E":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "0F":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "10":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "11":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "12":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "13":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "14":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "15":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "16":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "17":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;
                case "18":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Home;
                    break;         

                case "80":
                    goaltype = GoalType.ShortHanded2;
                    teamtype = HomeorAwayTeam.Away;
                    break;
                case "81":
                    goaltype = GoalType.ShortHanded;
                    teamtype = HomeorAwayTeam.Away;
                    break;
                case "82":
                    goaltype = GoalType.EvenStrength;
                    teamtype = HomeorAwayTeam.Away;
                    break;
                case "83":
                    goaltype = GoalType.PowerPlay;
                    teamtype = HomeorAwayTeam.Away;
                    break;
                case "84":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = HomeorAwayTeam.Away;
                    break;

                default:
                    teamtype = HomeorAwayTeam.Away;
                    break;
            }

            HomeorAwayandPlayerId results = new HomeorAwayandPlayerId();
            results.homeorawayteam = teamtype;
            results.PlayerRosterId = 1;
               
            //results.typeofgoal = goaltype;
            //results.homeorawayteam = teamtype;

            return results;

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
