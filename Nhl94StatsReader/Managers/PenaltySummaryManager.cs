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

        #endregion

        #region Constructors

        public PenaltySummaryManager(IStatReader Statreader, List<IStat> Stats)
        {
            _statreader = Statreader;
            _Stats = Stats;            
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
                var HomeorAwayTeam = GetHomeorAwayTeam(CurrentPenaltyOffset + 3);
                var TypeOfPenalty = GetPenaltyType(CurrentPenaltyOffset + 3);
                var TeamThatReceivedPenalty = GetTeamAbbrv(HomeorAwayTeam);
                var PlayerThatReceivedPenalty = GetPlayer(CurrentPenaltyOffset + 2, HomeorAwayTeam);
                var ps = new PenaltySummaryModel.PenaltySummary() { PenaltyType = TypeOfPenalty, Period = PeriodOfPenalty, Player = PlayerThatReceivedPenalty, Team = TeamThatReceivedPenalty, Time= TimeOfPenalty };
                PSM.Add(ps);

                CurrentPenaltyOffset += 4;

            }

            return PSM;
            //_boxscore.scoringsummary = SSM;

        }

        private PenaltyType GetPenaltyType(long Offset)
        {
            int result = _statreader.ReadStat(Offset);
            var penaltytype = new PenaltyType();

            switch (result)
            {
                case 20:
                    penaltytype = PenaltyType.Roughing;
                    break;
                case 24:
                    penaltytype = PenaltyType.Charging;
                    break;
                case 26:
                    penaltytype = PenaltyType.Slashing;
                    break;
                case 28:
                    penaltytype = PenaltyType.Roughing;
                    break;
                case 30:
                    penaltytype = PenaltyType.CrossCheck;
                    break;
                case 32:
                    penaltytype = PenaltyType.Hooking;
                    break;
                case 34:
                    penaltytype = PenaltyType.Tripping;
                    break;
                case 38:
                    penaltytype = PenaltyType.Interference;
                    break;
                case 40:
                    penaltytype = PenaltyType.Holding;
                    break;

                default:
                    //ERROR                   
                    break;
            }

            return penaltytype;

        }


        private HomeorAwayTeam GetHomeorAwayTeam(long Offset)
        {
            int result = _statreader.ReadStat(Offset);         
            HomeorAwayTeam teamtype;

            teamtype = (result > 24) ? HomeorAwayTeam.Away : HomeorAwayTeam.Home;

            return teamtype;
        }

        private string GetTeamAbbrv(HomeorAwayTeam HomeorAway)
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

        private string GetPlayer(long Offset, HomeorAwayTeam HomeorAway)
        {
            //todo There is duplicate code that can be broken out into more generic method

            int result = _statreader.ReadStat(Offset);            

            result = (result > 24) ? (result - 128) : result;

            var PlayerId = result;

            if (PlayerId == 255) return string.Empty;

            int TeamId = GetTeamId(HomeorAway);

            var playermodel = JsonConvert.DeserializeObject<Classic94PlayerModel>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json")));

            var getteams = playermodel.Select(x => x.Team).Distinct().ToList();

            var getteamabbrv = getteams[TeamId];

            var getplayer = (from p in playermodel where p.Team == getteamabbrv && p.RosterID == PlayerId + 1 select new { Name = p.FirstName + ", " + p.LastName }).FirstOrDefault();

            return getplayer.ToString();

        }



        private int GetPeriod(long Offset)
        {
            var result = _statreader.ReadStat(Offset);
            var period = result / 64 + 1;

            //LOG
            Console.WriteLine("Period: " + period);

            return period;
        }

        private string GetTime(long Offset)
        {

            var result = _statreader.ReadStat(Offset);
            var timespan = TimeSpan.FromSeconds(result);

            //LOG            
            Console.WriteLine(timespan);

            return timespan.ToString();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _statreader.Close();
                }

                _Stats = null;

                disposedValue = true;
            }
        }

        ~PenaltySummaryManager()
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
