using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    new IntegerStat(_statreader) { Offset = 9121, Statname = "Home Team Score", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9101, Statname = "Home Team Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9173, Statname = "Home Team Breakaways", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9177, Statname = "Home Team Breakaway Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9193, Statname = "Home Team One Timers", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9197, Statname = "Home Team One Timer Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9181, Statname = "Home Team Penalty Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9185, Statname = "Home Team Penalty Shot Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9125, Statname = "Home Team Faceoffs Won", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9161, Statname = "Home Team Body Checks", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9169, Statname = "Home Team Passes", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9165, Statname = "Home Team Passes Received", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9049, Statname = "Home Team Power Play", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9045, Statname = "Home Team Power Play Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9069, Statname = "Home Team Power Play Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9077, Statname = "Home Team Short Handed Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9053, Statname = "Home Team Penalties", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9057, Statname = "Home Team Penalty Time", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9085, Statname = "Home Team Period 1 Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9089, Statname = "Home Team Period 2 Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9093, Statname = "Home Team Period 3 Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9097, Statname = "Home Team Period OT Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9105, Statname = "Home Team Period 1 Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9109, Statname = "Home Team Period 2 Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9113, Statname = "Home Team Period 3 Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9117, Statname = "Home Team Period OT Goals", Stattype = StatType.Game},
                    new TimeStat(_statreader) { Offsets = new long[] { 9062, 9061 }, Statname = "Home Offensive Time Zone", Stattype = StatType.Game },
                    new TimeStat(_statreader) { Offsets = new long[] { 9066, 9065 }, Statname = "Home Power Play Time", Stattype = StatType.Game },

                    new IntegerStat(_statreader) { Offset = 9123, Statname = "Away Team Score", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9103, Statname = "Away Team Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9175, Statname = "Away Team Breakaways", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9179, Statname = "Away Team Breakaway Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9195, Statname = "Away Team One Timers", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9199, Statname = "Away Team One Timer Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9183, Statname = "Away Team Penalty Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9183, Statname = "Away Team Penalty Shot Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9127, Statname = "Away Team Faceoffs Won", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9163, Statname = "Away Team Body Checks", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9171, Statname = "Away Team Passes", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9167, Statname = "Away Team Passes Received", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9051, Statname = "Away Team Power Play", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9047, Statname = "Away Team Power Play Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9071, Statname = "Away Team Power Play Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9079, Statname = "Away Team Short Handed Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9055, Statname = "Away Team Penalties", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9059, Statname = "Away Team Penalty Time", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9087, Statname = "Away Team Period 1 Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9091, Statname = "Away Team Period 2 Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9095, Statname = "Away Team Period 3 Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9099, Statname = "Away Team Period OT Shots", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9107, Statname = "Away Team Period 1 Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9111, Statname = "Away Team Period 2 Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9115, Statname = "Away Team Period 3 Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9119, Statname = "Away Team Period OT Goals", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9687, Statname = "Crowd Meter Peak", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9689, Statname = "Crowd Meter Average", Stattype = StatType.Game},
                    new IntegerStat(_statreader) { Offset = 9685, Statname = "Crowd Meter Current", Stattype = StatType.Game},                    
                    new TimeStat(_statreader) { Offsets = new long[] { 9064, 9063 }, Statname = "Away Offensive Time Zone", Stattype = StatType.Game },
                    new TimeStat(_statreader) { Offsets = new long[] { 9068, 9067 }, Statname = "Away Power Play Time", Stattype = StatType.Game },
                    };

                ReadStats();
                
            }

            public void ReadStats()
            {
                foreach (IStat stat in _Stats)
                {
                    stat.ReadStat();
                }

            }
        
        #endregion

        
        
        
      
    }
}
