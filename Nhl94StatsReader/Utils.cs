using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Nhl94StatsReader
{
    public static class Utils
    {
        static Classic94PlayerModel _playermodel;

        public static int GetHomeTeamID() { return 0; }

        public static int GetAwayTeamID() { return 0; }

        public static string GetTeamAbbrv(int TeamID)
        {
            //Call GetClassic94PlayerModel or reseach to see if a variable is not set if it can be set by default in the get
            return "";
        }

        public static string GetHomeTeamAbbrv() { return string.Empty; }

        public static string GetAwayTeamAbbrv() { return string.Empty; }

        public static object GetAwayTeamRoster() { return string.Empty; }

        public static object GetHomeTeamRoster() { return string.Empty; }

        //public static void GetClassic94PlayerModel() { }

        public static Classic94PlayerModel GetClassic94PlayerModel()
        {
            _playermodel = (_playermodel == null) ? JsonConvert.DeserializeObject<Classic94PlayerModel>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json"))) : _playermodel;
            return _playermodel;
        }
    }
}
