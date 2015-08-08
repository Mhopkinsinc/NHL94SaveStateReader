using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nhl94StatsReader
{
    public class TeamStatManager : IDisposable
    {

        #region Properties

        IStatReader _statreader;

        #endregion

        #region Constructors

        public TeamStatManager(IStatReader Statreader)
        {
            _statreader = Statreader;            
        }

        #endregion

        #region Methods

        public TeamStatsModel GetTeamStats()
        {
            TeamStatsModel tsm = new TeamStatsModel();

            //Home Team Stats
            int teamid = _statreader.ReadStat(10411);
            string team = Utils.GetTeamAbbreviation(teamid);
            int score = _statreader.ReadStat(9121);
            int shots = _statreader.ReadStat(9101);
            int breakaways = _statreader.ReadStat(9173);
            int breakawygoals = _statreader.ReadStat(9177);
            int onetimers = _statreader.ReadStat(9193);
            int onetimergoals = _statreader.ReadStat(9197);
            int penaltyshots = _statreader.ReadStat(9181);
            int penaltyshotgoals = _statreader.ReadStat(9185);
            int faceoffsWon = _statreader.ReadStat(9125);
            int bodychecks = _statreader.ReadStat(9161);
            int passes = _statreader.ReadStat(9169);
            int passesreceived = _statreader.ReadStat(9165);
            int powerplays = _statreader.ReadStat(9049);
            int powerplaygoals = _statreader.ReadStat(9045);
            int powerplayshots = _statreader.ReadStat(9069);
            int shorthandedgoals = _statreader.ReadStat(9077);
            int penalities = _statreader.ReadStat(9053);
            int penaltytime = _statreader.ReadStat(9057);
            int period1shots = _statreader.ReadStat(9085);
            int period2shots = _statreader.ReadStat(9089);
            int period3shots = _statreader.ReadStat(9093);
            int periodotshots = _statreader.ReadStat(9097);
            int period1goals = _statreader.ReadStat(9105);
            int period2goals = _statreader.ReadStat(9109);
            int period3goals = _statreader.ReadStat(9113);
            int periodotgoals = _statreader.ReadStat(9117);
            string offensivetimezone = TimeSpan.FromSeconds(_statreader.ReadlLittleEndian(9061, 9062)).ToString(@"mm\:ss");
            string powerplaytime = TimeSpan.FromSeconds(_statreader.ReadlLittleEndian(9065, 9066)).ToString(@"mm\:ss");

            var homets = new TeamStatsModel.TeamStats() { Bodychecks = bodychecks, BreakawayGoals = breakawygoals, Breakaways = breakaways, FaceoffsWon = faceoffsWon, HomeOrAway = HomeorAwayTeam.Home, OffensiveTimeZone = offensivetimezone, OneTimerGoals = onetimergoals, OneTimers = onetimers, OTGoals = periodotgoals, OTShots = periodotshots, Passes = passes, PassesReceived = passesreceived, Penalities = penalities, PenaltyShotGoals = penaltyshotgoals, PenaltyShots = penaltyshots, PenaltyTime = penaltytime, Period1Goals = period1goals, Period1Shots = period1shots, Period2Goals = period2goals, Period2Shots = period2shots, Period3Goals = period3goals, Period3Shots = period3shots, PowerplayGoals = powerplaygoals, Powerplays = powerplays, PowerplayShots = powerplayshots, PowerplayTime = powerplaytime, Score = score, ShorthandedGoals = shorthandedgoals, Shots = shots, Team = team, TeamID = teamid   };
            tsm.Add(homets);

            //Away Team Stats
            teamid = _statreader.ReadStat(10413);
            team = Utils.GetTeamAbbreviation(teamid);
            score = _statreader.ReadStat(9123);
            shots = _statreader.ReadStat(9103);
            breakaways = _statreader.ReadStat(9175);
            breakawygoals = _statreader.ReadStat(9179);
            onetimers = _statreader.ReadStat(9195);
            onetimergoals = _statreader.ReadStat(9199);
            penaltyshots = _statreader.ReadStat(9183);
            penaltyshotgoals = _statreader.ReadStat(9187);
            faceoffsWon = _statreader.ReadStat(9127);
            bodychecks = _statreader.ReadStat(9163);
            passes = _statreader.ReadStat(9171);
            passesreceived = _statreader.ReadStat(9167);
            powerplays = _statreader.ReadStat(9051);
            powerplaygoals = _statreader.ReadStat(9047);
            powerplayshots = _statreader.ReadStat(9071);
            shorthandedgoals = _statreader.ReadStat(9079);
            penalities = _statreader.ReadStat(9055);
            penaltytime = _statreader.ReadStat(9059);
            period1shots = _statreader.ReadStat(9087);
            period2shots = _statreader.ReadStat(9091);
            period3shots = _statreader.ReadStat(9095);
            periodotshots = _statreader.ReadStat(9099);
            period1goals = _statreader.ReadStat(9107);
            period2goals = _statreader.ReadStat(9111);
            period3goals = _statreader.ReadStat(9115);
            periodotgoals = _statreader.ReadStat(9119);
            offensivetimezone = TimeSpan.FromSeconds(_statreader.ReadlLittleEndian(9063, 9064)).ToString(@"mm\:ss");
            powerplaytime = TimeSpan.FromSeconds(_statreader.ReadlLittleEndian(9067, 9068)).ToString(@"mm\:ss");

            var awayts = new TeamStatsModel.TeamStats() { Bodychecks = bodychecks, BreakawayGoals = breakawygoals, Breakaways = breakaways, FaceoffsWon = faceoffsWon, HomeOrAway = HomeorAwayTeam.Away, OffensiveTimeZone = offensivetimezone, OneTimerGoals = onetimergoals, OneTimers = onetimers, OTGoals = periodotgoals, OTShots = periodotshots, Passes = passes, PassesReceived = passesreceived, Penalities = penalities, PenaltyShotGoals = penaltyshotgoals, PenaltyShots = penaltyshots, PenaltyTime = penaltytime, Period1Goals = period1goals, Period1Shots = period1shots, Period2Goals = period2goals, Period2Shots = period2shots, Period3Goals = period3goals, Period3Shots = period3shots, PowerplayGoals = powerplaygoals, Powerplays = powerplays, PowerplayShots = powerplayshots, PowerplayTime = powerplaytime, Score = score, ShorthandedGoals = shorthandedgoals, Shots = shots, Team = team, TeamID = teamid };
            tsm.Add(awayts);

            return tsm;
        }      

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~TeamStatManager()
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

    }
}
