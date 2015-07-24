using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Nhl94StatsReader;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {

            //Stat Types
            // TimeStat, IntegerStat

            //Define The Stat in a list of stats
            // var hometeam = new stat

            //Set The Box Score Values
            // boxscore.hometeam = hometeam.readstat;

            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var statreader = new StatReader() { SaveStatePath = "mark"  };          
            
            var ts = new TimeStat() { Offsets = new long[] { 9062, 9061 }, Statname = "Offensive Time Zone", Stattype = StatType.Game,  _statreader = statreader };




            ts.ReadStat();
            

            Console.ReadLine();
             
          
            //CreateStats();
        }

        private static void CreateStats()
        {
            
                       
            //Create Stat Manager
            var sm = new StatManager();

            //Create Default Stats
            List<IStat> stats = DefaultStats();           
          

            //Create Stat Reader
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            //var reader = new StatReader(path); //@"C:\Users\Mark\Desktop\nhl94.zs3");
            
            //Read List Of Stats
            //reader.ReadStats(stats);
            Console.ReadLine();
            
        }

        private static List<IStat> DefaultStats()
        {
            List<IStat> stats = new List<IStat>
            {
            //new Stat() { Offset = 10411, Statname = "Home Team Selected", StatType = StatType.Game },            
            //new Stat() { Offset = 9121 ,Statname = "Home Score", StatType = StatType.Game},
            //new Stat() { Offset = 9101, Statname = "Home Shots", StatType = StatType.Game },
            //new Stat() { Offset = 9161, Statname = "Home Body Checks", StatType = StatType.Game },            
            //new Stat() { Offset = 9061, Statname = "Home Team Attack Zone 1", StatType = StatType.Game },
            //new Stat() { Offset = 9062, Statname = "Home Team Attack Zone 2", StatType = StatType.Game },
            //new Stat() { Offset = 10413, Statname = "Away Team Selected", StatType = StatType.Game }                   
            };
            return stats;
        }


    }
}
