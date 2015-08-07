using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nhl94StatsReader
{
    public class PlayerStatManager : IDisposable
    {
        #region Properties

        IStatReader _statreader;
        List<IStat> _stats;
        Classic94PlayerModel _playermodel;

        #endregion

        #region Constructors

        public PlayerStatManager(IStatReader Statreader, List<IStat> Stats)
        {
            _statreader = Statreader;
            _stats = Stats;
        }

        #endregion

        #region Methods

        public PlayerStatsModel GetPlayerStats()
        {
            PlayerStatsModel psm = new PlayerStatsModel();

            var hometeam = GetTeamAbbrv(HomeorAwayTeam.Home);
            var awayteam = GetTeamAbbrv(HomeorAwayTeam.Away);
            var playermodel = Create94ClassicPlayerModel();
            var HomeTeamPlayers = from p in playermodel where p.Team == hometeam select p;
            var AwayTeamPlayers = from p in playermodel where p.Team == awayteam select p;            

            //Get The HomeTeam Roster, Then For Each Player Get All The Player Stats
            foreach (var player in HomeTeamPlayers)
            {
                var goals = _statreader.ReadStat(9472 + player.RosterID );
                var assists = _statreader.ReadStat(9524 + player.RosterID);
                var points = goals + assists;
                var shots = _statreader.ReadStat(9576 + player.RosterID);
                var pim = _statreader.ReadStat(9628 + player.RosterID);
                var playername = player.FirstName + " " + player.LastName;
                var ps = new PlayerStatsModel.PlayerStats() { Assissts = assists, Goals = goals, PenaltyMinutes = pim, Player = playername, Team = hometeam, Points = points, Shots = shots };
                psm.Add(ps);
            }

            //Get The AwayTeam Roster, Then For Each Player Get All The Player Stats
            foreach (var player in AwayTeamPlayers)
            {
                var goals = _statreader.ReadStat(9498 + player.RosterID);
                var assists = _statreader.ReadStat(9550 + player.RosterID);
                var points = goals + assists;
                var shots = _statreader.ReadStat(9602 + player.RosterID);
                var pim = _statreader.ReadStat(9654 + player.RosterID);
                var playername = player.FirstName + " " + player.LastName;
                var ps = new PlayerStatsModel.PlayerStats() { Assissts = assists, Goals = goals, PenaltyMinutes = pim, Player = playername, Team = awayteam, Points = points, Shots = shots };
                psm.Add(ps);
            }

            return psm;
        }

        private int GetPeriod(long Offset)
        {
            var result = _statreader.ReadStat(Offset);
            var period = result / 64 + 1;

            //LOG
            Console.WriteLine("Period: " + period);

            return period;
        }

        private Classic94PlayerModel Create94ClassicPlayerModel()
        {
            _playermodel = (_playermodel == null) ? JsonConvert.DeserializeObject<Classic94PlayerModel>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json"))) : _playermodel;
            return _playermodel;
        }

        private string GetTeamAbbrv(HomeorAwayTeam HomeorAway)
        {

            int TeamId = GetTeamId(HomeorAway);

            var playermodel = Create94ClassicPlayerModel();

            var getteams = playermodel.Select(x => x.Team).Distinct().ToList();

            var getteamabbrv = getteams[TeamId];

            return getteamabbrv;
        }

        private int GetTeamId(HomeorAwayTeam HomeorAway)
        {
            int TeamId;

            // Get All IntegerStats from _Stats
            var IntStats = from p in _stats
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
        #endregion



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

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _stats = null;
                _playermodel = null;

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
