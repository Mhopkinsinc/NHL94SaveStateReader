using System;
using NLog;


namespace Nhl94StatsReader
{
    public class PenaltySummaryManager : IDisposable
    {
     
        //TODO: Problem When The Penalty happens at the 5:00 minute mark it is showing the wrong time?
           
        #region Properties

        IStatReader _statreader;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        public PenaltySummaryManager(IStatReader Statreader)
        {
            if (Statreader == null) { logger.Error("Statreader was null"); throw new ArgumentNullException("Statreader Can't be null."); }
            _statreader = Statreader;
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
                var TeamId = GetTeamID(HomeorAwayTeam);
                var TeamAbbreviation = Utils.GetTeamAbbreviation(TeamId);
                var TypeOfPenalty = GetPenaltyType(CurrentPenaltyOffset + 3);                
                var PlayerThatReceivedPenalty = GetPlayer(CurrentPenaltyOffset + 2, TeamAbbreviation);
                var ps = new PenaltySummaryModel.PenaltySummary() { PenaltyType = TypeOfPenalty, Period = PeriodOfPenalty, Player = PlayerThatReceivedPenalty, Team = TeamAbbreviation, Time= TimeOfPenalty };
                PSM.Add(ps);

                CurrentPenaltyOffset += 4;

            }

            return PSM;           

        }
        private int GetTeamID(HomeorAwayTeam HomeorAway)
        {
            int teamid;

            teamid = (HomeorAway == HomeorAwayTeam.Home) ? _statreader.ReadStat(10411) : _statreader.ReadStat(10413);

            return teamid;

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

        private string GetPlayer(long offset, string TeamAbbreviation)
        {
            int rosterid = _statreader.ReadStat(offset);

            rosterid = (rosterid > 24) ? (rosterid - 128) : rosterid;

            if (rosterid == 255) return string.Empty;

            var player = Utils.GetPlayer(TeamAbbreviation, rosterid);

            return player;
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
