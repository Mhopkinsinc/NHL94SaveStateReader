using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Nhl94StatsReader
{
    public class StatManager
    {

        #region Properties
        
            List<IStat> _Stats;
            IStatReader _statreader;

        #endregion

        #region Constructors

            public StatManager()
            { }

            public StatManager(String SaveStatePath)
            {
                CreateStatReader(SaveStatePath);
            }
        
        #endregion

        #region Methods

            public void AddStat(IStat stat)
            {
                _Stats.Add(stat);
            }

            public void LoadSaveState(String SaveStatePath)
            {
                CreateStatReader(SaveStatePath);
            }            
        
            private void CreateStatReader(String SaveStatePath)
            {
               _statreader = new StatReader(SaveStatePath); 
            }

            public void LoadDefaultStats()
            {
                _Stats = new List<IStat>
                    {
                    //HOME TEAM STATS
                    new IntegerStat(_statreader) { Offset = 10411, Statname = "Home Team ID", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9121, Statname = "Home Team Score", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9101, Statname = "Home Team Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9173, Statname = "Home Team Breakaways", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9177, Statname = "Home Team Breakaway Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9193, Statname = "Home Team One Timers", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9197, Statname = "Home Team One Timer Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9181, Statname = "Home Team Penalty Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9185, Statname = "Home Team Penalty Shot Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9125, Statname = "Home Team Faceoffs Won", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9161, Statname = "Home Team Body Checks", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9169, Statname = "Home Team Passes", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9165, Statname = "Home Team Passes Received", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9049, Statname = "Home Team Power Play", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9045, Statname = "Home Team Power Play Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9069, Statname = "Home Team Power Play Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9077, Statname = "Home Team Short Handed Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9053, Statname = "Home Team Penalties", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9057, Statname = "Home Team Penalty Time", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9085, Statname = "Home Team Period 1 Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9089, Statname = "Home Team Period 2 Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9093, Statname = "Home Team Period 3 Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9097, Statname = "Home Team Period OT Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9105, Statname = "Home Team Period 1 Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9109, Statname = "Home Team Period 2 Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9113, Statname = "Home Team Period 3 Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9117, Statname = "Home Team Period OT Goals", Stattype = StatType.Team},
                    new TimeStat(_statreader) { Offsets = new long[] { 9062, 9061 }, Statname = "Home Offensive Time Zone", Stattype = StatType.Team },
                    new TimeStat(_statreader) { Offsets = new long[] { 9066, 9065 }, Statname = "Home Power Play Time", Stattype = StatType.Team },
                    //AWAY TEAM STATS
                    new IntegerStat(_statreader) { Offset = 10413, Statname = "Away Team ID", Stattype = StatType.Team},                    
                    new IntegerStat(_statreader) { Offset = 9123, Statname = "Away Team Score", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9103, Statname = "Away Team Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9175, Statname = "Away Team Breakaways", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9179, Statname = "Away Team Breakaway Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9195, Statname = "Away Team One Timers", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9199, Statname = "Away Team One Timer Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9183, Statname = "Away Team Penalty Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9183, Statname = "Away Team Penalty Shot Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9127, Statname = "Away Team Faceoffs Won", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9163, Statname = "Away Team Body Checks", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9171, Statname = "Away Team Passes", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9167, Statname = "Away Team Passes Received", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9051, Statname = "Away Team Power Play", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9047, Statname = "Away Team Power Play Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9071, Statname = "Away Team Power Play Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9079, Statname = "Away Team Short Handed Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9055, Statname = "Away Team Penalties", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9059, Statname = "Away Team Penalty Time", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9087, Statname = "Away Team Period 1 Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9091, Statname = "Away Team Period 2 Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9095, Statname = "Away Team Period 3 Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9099, Statname = "Away Team Period OT Shots", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9107, Statname = "Away Team Period 1 Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9111, Statname = "Away Team Period 2 Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9115, Statname = "Away Team Period 3 Goals", Stattype = StatType.Team},
                    new IntegerStat(_statreader) { Offset = 9119, Statname = "Away Team Period OT Goals", Stattype = StatType.Team},                              
                    new TimeStat(_statreader) { Offsets = new long[] { 9064, 9063 }, Statname = "Away Offensive Time Zone", Stattype = StatType.Team },
                    new TimeStat(_statreader) { Offsets = new long[] { 9068, 9067 }, Statname = "Away Power Play Time", Stattype = StatType.Team },
                    //CROWD METER
                    new IntegerStat(_statreader) { Offset = 9687, Statname = "Crowd Meter Peak", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9689, Statname = "Crowd Meter Average", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9685, Statname = "Crowd Meter Current", Stattype = StatType.Game},  
                    };

            ReadStats();
            GetScoringSummary();
                
            }

            public void ReadStats()
            {

                foreach (IStat stat in _Stats)
                {
                    stat.ReadStat();
                }

            }

        public void GetScoringSummary()
        {

            var GoalsScored = _statreader.ReadStat(15693)/6;

            var time = GetTimeOfGoal(15695);
            var period = GetPeriodOfGoal(15696);
            var goaltypeandteam = GetGoalType(15697);
            var goalscorer = GetGoalScorer(15698, goaltypeandteam.Item2);

            //for (int i = 1; i < 10; i++)
            //{
            //    var startingoffset = (15694);            
            //}

        }

        internal string GetGoalScorer(long Offset, TeamType HomeorAway)
        {
            var PlayerId = _statreader.ReadStat(Offset);

            // Get All IntegerStats from _Stats
            var IntStats = from p in _Stats
                           where p.GetType() == typeof(IntegerStat)
                           select p;
            int TeamId;

            //
            switch (HomeorAway)
            {                
                case TeamType.Home:
                    TeamId = (from IntegerStat p in IntStats
                     where p.Statname == "Home Team ID"
                     select p._statValueInt).FirstOrDefault();
                    break;
                case TeamType.Away:
                    TeamId = (from IntegerStat p in IntStats
                     where p.Statname == "Away Team ID"
                     select p._statValueInt).FirstOrDefault();
                    break;
                default:
                    TeamId = 0;
                    break;
            }
                                                

            var playermodel = JsonConvert.DeserializeObject<Classic94PlayerModel >(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json")));

            var getteams = playermodel.Select(x => x.Team).Distinct().ToList();

            var getteamabbrv = getteams[TeamId];

            var getplayer = (from p in playermodel where p.Team == getteamabbrv && p.RosterID == PlayerId+1 select new { Name = p.FirstName + ", " + p.LastName }).FirstOrDefault();

            return getplayer.ToString();
           
        }

        internal Tuple<GoalType,TeamType> GetGoalType (long Offset)
        {
            var result = _statreader.ReadStat(Offset).ToString("X");
            var goaltype = new GoalType();
            TeamType teamtype;

            switch (result)
            {
                case "00":
                    goaltype = GoalType.ShortHanded2;
                    teamtype = TeamType.Home;
                    break;
                case "01":
                    goaltype = GoalType.ShortHanded;
                    teamtype = TeamType.Home;
                    break;
                case "02":
                    goaltype = GoalType.EvenStrength;
                    teamtype = TeamType.Home;
                    break;
                case "03":
                    goaltype = GoalType.PowerPlay;
                    teamtype = TeamType.Home;
                    break;
                case "04":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = TeamType.Home;
                    break;

                case "80":
                    goaltype = GoalType.ShortHanded2;
                    teamtype = TeamType.Away;
                    break;
                case "81":
                    goaltype = GoalType.ShortHanded;
                    teamtype = TeamType.Away;
                    break;
                case "82":
                    goaltype = GoalType.EvenStrength;
                    teamtype = TeamType.Away;
                    break;
                case "83":
                    goaltype = GoalType.PowerPlay;
                    teamtype = TeamType.Away;
                    break;
                case "84":
                    goaltype = GoalType.PowerPlay2;
                    teamtype = TeamType.Away;
                    break;

                default:
                    teamtype = TeamType.Away;   
                    break;
            }

            return Tuple.Create(goaltype, teamtype);

        }

        internal int GetPeriodOfGoal (long Offset)
        {
            var result = _statreader.ReadStat(Offset);
            var period = result / 64 + 1;            
            
            //LOG
            Console.WriteLine("Period: " + period);

            return period;
        }

        internal string GetTimeOfGoal(long Offset)
        {
                                               
            var result = _statreader.ReadStat(Offset);
            var timespan = TimeSpan.FromSeconds(result);

            //LOG            
            Console.WriteLine(timespan);

            return timespan.ToString();
        }

        public void ReferenceLinq()
        {
            //---------------------------------------------------------
            //THIS CODE WAS USED TO PERFORM A GROUP BY TO ADD A ROSTER POSITION ID BASED ON CHAOS CSV FILE ROSTER CREATION OUTPUT.
            //---------------------------------------------------------
            //var temp = JsonConvert.DeserializeObject<Classic94PlayerModel >(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Json\Classic94Players.json")));

            //var o = temp.OrderBy(x => x.PlayerID).GroupBy(x => x.Team)
            //.Select(g => new { g, count = g.Count() })
            //.SelectMany(t => t.g.Select(b => b)
            //.Zip(Enumerable.Range(1, t.count), (j, i) => new { PlayerID = j.PlayerID, j.FirstName, j.LastName, j.Team, j.Position, j.Jersey, RosterID = i }));

            //var tempjson = JsonConvert.SerializeObject(o);

            //var HomeTeamPlayers = from p in temp where p.Team == "CGY" select p;
            
            //---------------------------------------------------------

            //---------------------------------------------------------
            //THIS CODE WAS USED TO QUERY THE _STATS ARRAY 
            //---------------------------------------------------------
            
            //var IntStats = from p in _Stats
            //               where p.GetType() == typeof(IntegerStat)
            //               select p;

            //var myquery = (from IntegerStat p in IntStats
            //               where p.Statname == "Home Team Score" || p.Statname == "Away Team Score"
            //              select p._statValueInt).Sum();            

            //var TotalGoals = _Stats.Where(a => a.GetType == typeof(IntegerStat)).Select(a => a.)
            //var TotalGoals = _Stats.First<IntegerStat>(a => a.Statname == "Home Team Score");
           
            //---------------------------------------------------------
        }

        #endregion


    }
}
