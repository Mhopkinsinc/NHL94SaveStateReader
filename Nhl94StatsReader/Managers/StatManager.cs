using System;
using NLog;
using System.IO;
using System.Linq;

namespace Nhl94StatsReader
{
    
    //TODO : Add Crowd Meter Stats To A MISC STAT MANAGER & CROWD METER MODEL?
    
    //[
    //    Category("Main"),
    //    Description("Text"),        
    //    Browsable(false),
    //    EditorBrowsable(EditorBrowsableState.Never)
    //]
    public class StatManager : IDisposable
    {

        #region Properties
                
        IStatReader _statreader;
        Boxscore _boxscore;
        string _savestatepath = "";

        private static Logger logger = LogManager.GetCurrentClassLogger();                

        #endregion

        #region Constructors        

        public StatManager(String SaveStatePath)
        {
            if (string.IsNullOrEmpty(SaveStatePath)) { logger.Error("SaveStatePath was null or empty."); throw new ArgumentNullException("SaveStatePath Can't be Null or Empty String. "); }
            if (SaveStatePath.LastIndexOfAny(Path.GetInvalidPathChars()) >= 0) { logger.Error("Invalid Path Characters In SaveStatePath=\"{0}\")", SaveStatePath); throw new ArgumentException("Invalid Characters In Path"); }
            if (SaveStatePath.Any( x => char.IsWhiteSpace ( x ))) { logger.Error("Whitespace Characters In SaveStatePath=\"{0}\" ", SaveStatePath); throw new ArgumentException("WhiteSpace Characters In Path"); }

            logger.Info("Called with (SaveStatePath=\"{0}\")", SaveStatePath);
            LoadSaveState(SaveStatePath);
            _savestatepath = SaveStatePath;
        }

        #endregion

        #region Methods        

        private void LoadSaveState(String SaveStatePath)
        {
            logger.Info("Called with (SaveStatePath=\"{0}\")", SaveStatePath);
            CreateStatReader(SaveStatePath);
        }

        private void CreateStatReader(String SaveStatePath)
        {
            logger.Info("Called with (SaveStatePath=\"{0}\")", SaveStatePath);
            if (_savestatepath != SaveStatePath && _statreader != null) { _statreader.Close(); _statreader = null; _boxscore = null; logger.Info("Creating New StatReader As A New Save State Files Has Been Passed In."); }
            _statreader = (_statreader == null) ? _statreader = new StatReader(SaveStatePath) : _statreader;            
            _boxscore = (_boxscore == null) ? _boxscore = new Boxscore() : _boxscore;
            _savestatepath = SaveStatePath;
        }

        public Boxscore GenerateBoxScore()
        {

            logger.Info("Generating BoxScore");
            GetTeamStats();
            GetScoringSummary();
            GetPenaltySummary();
            GetPlayerStats();

            return _boxscore;
        }

        private void GetTeamStats()
        {
            var TSM = new TeamStatManager(_statreader);
            var TSmodel = TSM.GetTeamStats();
            _boxscore.teamstats = TSmodel;
            TSM.Dispose();
        }

        private void GetPlayerStats()
        {
            var PSM = new PlayerStatManager(_statreader);
            var PSmodel = PSM.GetPlayerStats();
            _boxscore.playerstats = PSmodel;
            PSM.Dispose();
        }

        private void GetPenaltySummary()
        {
            var PSM = new PenaltySummaryManager(_statreader);
            var PSmodel = PSM.GetPenaltySummary();
            _boxscore.penaltysummary = PSmodel;
            PSM.Dispose();
        }


        private void GetScoringSummary()
        {
            var SSM = new ScoringSummaryManager(_statreader);
            var SSmodel = SSM.GetScoringSummary();
            _boxscore.scoringsummary = SSmodel;
            SSM.Dispose();
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

                _boxscore = null;                

                disposedValue = true;
            }
        }

        ~StatManager()
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
            //GC.SuppressFinalize(this);
        }
        #endregion

        #region Obsolete + Old Reference Code

        //[Obsolete("Do not call this method.")]
        //private void GetTeamStats()
        //{
        //    _stats = new List<IStat>
        //            {
        //            //HOME TEAM STATS
        //            new IntegerStat(_statreader) { Offset = 10411, Statname = "Home Team ID", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9121, Statname = "Home Team Score", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9101, Statname = "Home Team Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9173, Statname = "Home Team Breakaways", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9177, Statname = "Home Team Breakaway Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9193, Statname = "Home Team One Timers", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9197, Statname = "Home Team One Timer Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9181, Statname = "Home Team Penalty Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9185, Statname = "Home Team Penalty Shot Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9125, Statname = "Home Team Faceoffs Won", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9161, Statname = "Home Team Body Checks", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9169, Statname = "Home Team Passes", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9165, Statname = "Home Team Passes Received", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9049, Statname = "Home Team Power Play", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9045, Statname = "Home Team Power Play Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9069, Statname = "Home Team Power Play Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9077, Statname = "Home Team Short Handed Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9053, Statname = "Home Team Penalties", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9057, Statname = "Home Team Penalty Time", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9085, Statname = "Home Team Period 1 Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9089, Statname = "Home Team Period 2 Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9093, Statname = "Home Team Period 3 Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9097, Statname = "Home Team Period OT Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9105, Statname = "Home Team Period 1 Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9109, Statname = "Home Team Period 2 Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9113, Statname = "Home Team Period 3 Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9117, Statname = "Home Team Period OT Goals", Stattype = StatType.Team},
        //            new TimeStat(_statreader) { Offsets = new long[] { 9062, 9061 }, Statname = "Home Offensive Time Zone", Stattype = StatType.Team },
        //            new TimeStat(_statreader) { Offsets = new long[] { 9066, 9065 }, Statname = "Home Power Play Time", Stattype = StatType.Team },
        //            //AWAY TEAM STATS
        //            new IntegerStat(_statreader) { Offset = 10413, Statname = "Away Team ID", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9123, Statname = "Away Team Score", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9103, Statname = "Away Team Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9175, Statname = "Away Team Breakaways", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9179, Statname = "Away Team Breakaway Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9195, Statname = "Away Team One Timers", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9199, Statname = "Away Team One Timer Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9183, Statname = "Away Team Penalty Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9183, Statname = "Away Team Penalty Shot Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9127, Statname = "Away Team Faceoffs Won", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9163, Statname = "Away Team Body Checks", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9171, Statname = "Away Team Passes", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9167, Statname = "Away Team Passes Received", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9051, Statname = "Away Team Power Play", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9047, Statname = "Away Team Power Play Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9071, Statname = "Away Team Power Play Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9079, Statname = "Away Team Short Handed Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9055, Statname = "Away Team Penalties", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9059, Statname = "Away Team Penalty Time", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9087, Statname = "Away Team Period 1 Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9091, Statname = "Away Team Period 2 Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9095, Statname = "Away Team Period 3 Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9099, Statname = "Away Team Period OT Shots", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9107, Statname = "Away Team Period 1 Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9111, Statname = "Away Team Period 2 Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9115, Statname = "Away Team Period 3 Goals", Stattype = StatType.Team},
        //            new IntegerStat(_statreader) { Offset = 9119, Statname = "Away Team Period OT Goals", Stattype = StatType.Team},
        //            new TimeStat(_statreader) { Offsets = new long[] { 9064, 9063 }, Statname = "Away Offensive Time Zone", Stattype = StatType.Team },
        //            new TimeStat(_statreader) { Offsets = new long[] { 9068, 9067 }, Statname = "Away Power Play Time", Stattype = StatType.Team },
        //            //CROWD METER
        //            new IntegerStat(_statreader) { Offset = 9687, Statname = "Crowd Meter Peak", Stattype = StatType.Game},
        //            new IntegerStat(_statreader) { Offset = 9689, Statname = "Crowd Meter Average", Stattype = StatType.Game},
        //            new IntegerStat(_statreader) { Offset = 9685, Statname = "Crowd Meter Current", Stattype = StatType.Game},
        //            };

        //    ReadStats();

        //}       

        #endregion


    }
}
