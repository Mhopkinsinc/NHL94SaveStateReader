using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Nhl94StatsReader
{
    public static class Utils
    {
        private static Classic94PlayerModel _playermodel;
        private static List<String> _teams;

        public static Classic94PlayerModel Playermodel { get { return _playermodel ?? (_playermodel = GetClassic94PlayerModel()); } }

        public static List<String> Teams { get { return _teams ?? (_teams = GetTeams()); } }            
        
        public static IEnumerable<Classic94PlayerModel.Classic94Player> GetTeamRoster(string TeamAbbreviation)
        {
            var results = from p in _playermodel where p.Team == TeamAbbreviation select p;
            return results;
        }

        public static string GetTeamAbbreviation(int TeamID)
        {
            return Teams[TeamID];
        }

        public static string GetPlayer(string TeamAbbreviation, int RosterId)
        {
            var getplayer = (from p in _playermodel where p.Team == TeamAbbreviation && p.RosterID == RosterId + 1 select new { Name = p.FirstName + ", " + p.LastName }).FirstOrDefault();
            return getplayer.ToString();
        }

        private static List<String> GetTeams()
        {
            return Playermodel.Select(x => x.Team).Distinct().ToList();
        }        

        private static Classic94PlayerModel GetClassic94PlayerModel()
        {            
            return JsonConvert.DeserializeObject<Classic94PlayerModel>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json")));
        }
    }
}
