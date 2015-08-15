using System;
using NLog;


namespace Nhl94StatsReader
{
    public class PlayerStatManager : IDisposable
    {
        //TODO : Add Additonal Calculated stats like save percent shooting percent
        
        #region Properties

        IStatReader _statreader;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        public PlayerStatManager(IStatReader Statreader)
        {
            if (Statreader == null) { logger.Error("Statreader was null"); throw new ArgumentNullException("Statreader Can't be null."); }
            _statreader = Statreader;
        }

        #endregion

        #region Methods

        public PlayerStatsModel GetPlayerStats()
        {
            PlayerStatsModel psm = new PlayerStatsModel();

            var hometeamid = GetTeamID(HomeorAwayTeam.Home);
            var awayteamid = GetTeamID(HomeorAwayTeam.Away);
            var hometeamaabbreviation = Utils.GetTeamAbbreviation(hometeamid);
            var awayteamaabbreviation = Utils.GetTeamAbbreviation(awayteamid);
            
            //Get The HomeTeam Roster, Then For Each Player Get All The Player Stats
            foreach (var player in Utils.GetTeamRoster(hometeamaabbreviation))
            {
                if (player.Position == "G")
                {
                    var goalsagainst = _statreader.ReadStat(9472 + player.RosterID);
                    var assists = _statreader.ReadStat(9524 + player.RosterID);
                    var points = assists;
                    var shotsagainst = _statreader.ReadStat(9576 + player.RosterID);
                    var playername = player.FirstName + " " + player.LastName;
                    var ps = new PlayerStatsModel.PlayerStats() { Assissts = assists, GoalsAgainst = goalsagainst, Player = playername, Team = awayteamaabbreviation, Points = points, ShotsAgainst = shotsagainst };
                    psm.Add(ps);
                }
                else
                {
                    var goals = _statreader.ReadStat(9472 + player.RosterID);
                    var assists = _statreader.ReadStat(9524 + player.RosterID);
                    var points = goals + assists;
                    var shots = _statreader.ReadStat(9576 + player.RosterID);
                    var pim = _statreader.ReadStat(9628 + player.RosterID);
                    var playername = player.FirstName + " " + player.LastName;
                    var ps = new PlayerStatsModel.PlayerStats() { Assissts = assists, Goals = goals, PenaltyMinutes = pim, Player = playername, Team = hometeamaabbreviation, Points = points, Shots = shots };
                    psm.Add(ps);
                }

                
            }

            //Get The AwayTeam Roster, Then For Each Player Get All The Player Stats
            foreach (var player in Utils.GetTeamRoster(awayteamaabbreviation))
            {
                if (player.Position == "G")
                {
                    var goalsagainst = _statreader.ReadStat(9498 + player.RosterID);
                    var assists = _statreader.ReadStat(9550 + player.RosterID);
                    var points = assists;
                    var shotsagainst = _statreader.ReadStat(9602 + player.RosterID);                    
                    var playername = player.FirstName + " " + player.LastName;
                    var ps = new PlayerStatsModel.PlayerStats() { Assissts = assists, GoalsAgainst = goalsagainst, Player = playername, Team = awayteamaabbreviation, Points = points, ShotsAgainst = shotsagainst };
                    psm.Add(ps);
                }
                else
                {
                    var goals = _statreader.ReadStat(9498 + player.RosterID);
                    var assists = _statreader.ReadStat(9550 + player.RosterID);
                    var points = goals + assists;
                    var shots = _statreader.ReadStat(9602 + player.RosterID);
                    var pim = _statreader.ReadStat(9654 + player.RosterID);
                    var playername = player.FirstName + " " + player.LastName;
                    var ps = new PlayerStatsModel.PlayerStats() { Assissts = assists, Goals = goals, PenaltyMinutes = pim, Player = playername, Team = awayteamaabbreviation, Points = points, Shots = shots };
                    psm.Add(ps);
                }
            }

            return psm;
        }

        private int GetPeriod(long Offset)
        {
            var result = _statreader.ReadStat(Offset);
            var period = result / 64 + 1;

            return period;
        }   

        private int GetTeamID(HomeorAwayTeam HomeorAway)
        {
            int teamid;

            teamid = (HomeorAway == HomeorAwayTeam.Home) ? _statreader.ReadStat(10411) : _statreader.ReadStat(10413);

            return teamid;

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
                    _statreader = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.                

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~PlayerStatManager()
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
