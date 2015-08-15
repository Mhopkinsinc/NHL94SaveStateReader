using Xunit;
using Nhl94StatsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Nhl94StatsReader.Tests
{
    public class TeamStatManagerTests
    {
        [Fact()]
        public void TeamStatManagerTest()
        {

            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var TSM = new TeamStatManager(_statreader);                    
            
            Assert.True(TSM != null, "TestStat Manager Created");
        }

        [Fact()]
        public void GetTeamStatsTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var TSM = new TeamStatManager(_statreader);
            var TSmodel = TSM.GetTeamStats();
            var _boxscore = new Boxscore();
            _boxscore.teamstats = TSmodel;

            Assert.True(_boxscore.teamstats != null, "TeamStats Model & BoxScore Created");
            
        }

        [Fact()]
        public void DisposeTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var TSM = new TeamStatManager(_statreader);
            var TSmodel = new TeamStatsModel();

            //Call Dispose
            TSM.Dispose();

            //Calling The GetTeamStats Method should fail as the FileStream Reader should be null.
            Exception ex = Assert.Throws<System.NullReferenceException>(() => TSmodel = TSM.GetTeamStats());

            //Verify The Expected Exception was thrown.
            Assert.Equal("Object reference not set to an instance of an object.", ex.Message);            

        }
    }
}