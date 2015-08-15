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
    public class PenaltySummaryManagerTests
    {
        [Fact()]
        public void PenaltySummaryManagerTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var PSM = new PenaltySummaryManager(_statreader);

            Assert.True(PSM != null, "PenaltySummary Manager Created");
        }

        [Fact()]
        public void GetPenaltySummaryTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var PSM = new PenaltySummaryManager(_statreader);
            var PSmodel = PSM.GetPenaltySummary();
            var _boxscore = new Boxscore();
            _boxscore.penaltysummary = PSmodel;

            Assert.True(_boxscore.penaltysummary != null, "PenaltySummary Model & Boxscore Created");

        }

        [Fact()]
        public void DisposeTest()
        {
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            var _statreader = new StatReader(_saveStatePath);

            var PSM = new PenaltySummaryManager(_statreader);
            var PSmodel = new PenaltySummaryModel();

            //Call Dispose
            PSM.Dispose();

            //Calling The GetPenaltySummary Method should fail as the FileStream Reader should be null.
            Exception ex = Assert.Throws<System.NullReferenceException>(() => PSmodel = PSM.GetPenaltySummary());

            //Verify The Expected Exception was thrown.
            Assert.Equal("Object reference not set to an instance of an object.", ex.Message);
        }
    }
}