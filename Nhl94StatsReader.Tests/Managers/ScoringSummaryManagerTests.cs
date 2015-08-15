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
    public class ScoringSummaryManagerTests
    {
        [Fact()]
        public void ScoringSummaryManagerTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var SSM = new ScoringSummaryManager(_statreader);

            Assert.True(SSM != null, "ScoringSummary Manager Created");

        }

        [Fact()]
        public void GetScoringSummaryTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var SSM = new ScoringSummaryManager(_statreader);
            var SSmodel = SSM.GetScoringSummary();
            var _boxscore = new Boxscore();
            _boxscore.scoringsummary = SSmodel;

            Assert.True(_boxscore.scoringsummary != null, "ScoringSummary Model & Boxscore Created");
        }

        [Fact()]
        public void DisposeTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var SSM = new ScoringSummaryManager(_statreader);
            var SSmodel = new ScoringSummaryModel();

            //Call Dispose
            SSM.Dispose();

            //Calling The GetScoringSummary Method should fail as the FileStream Reader should be null.
            Exception ex = Assert.Throws<System.NullReferenceException>(() => SSmodel = SSM.GetScoringSummary());

            //Verify The Expected Exception was thrown.
            Assert.Equal("Object reference not set to an instance of an object.", ex.Message);
        }
    }
}