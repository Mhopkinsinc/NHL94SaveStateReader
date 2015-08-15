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
    public class PlayerStatManagerTests
    {
        [Fact()]
        public void PlayerStatManagerTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var PSM = new PlayerStatManager(_statreader);

            Assert.True(PSM != null, "PlayerStat Manager Created");

        }

        [Fact()]
        public void GetPlayerStatsTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var PSM = new PlayerStatManager(_statreader);
            var PSmodel = PSM.GetPlayerStats();
            var _boxscore = new Boxscore();
            _boxscore.playerstats = PSmodel;

            Assert.True(_boxscore.playerstats != null, "PlayerStats Model & Boxscore Created");
        }

        [Fact()]
        public void DisposeTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var PSM = new PlayerStatManager(_statreader);
            var PSmodel = new PlayerStatsModel();

            //Call Dispose
            PSM.Dispose();

            //Calling The GetScoringSummary Method should fail as the FileStream Reader should be null.
            Exception ex = Assert.Throws<System.NullReferenceException>(() => PSmodel = PSM.GetPlayerStats());

            //Verify The Expected Exception was thrown.
            Assert.Equal("Object reference not set to an instance of an object.", ex.Message);


        }
    }
}