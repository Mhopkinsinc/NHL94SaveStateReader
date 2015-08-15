using Xunit;
using Nhl94StatsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit.Abstractions;

namespace Nhl94StatsReader.Tests
{
    public class StatManagerTests
    {
        

        [Fact()]
        public void StatManagerTest()
        {
            //Set The Path Of The ZSNES Save State File            
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");

            //Create An Instance Of The Stat Manager Passing In The Path Of The ZSNES Save State.            
            var sm = new StatManager(_saveStatePath);            

            Assert.True(sm!=null, "My Message");
        }

        [Fact()]
        public void GenerateBoxScoreTest()
        {
            //Set The Path Of The ZSNES Save State File            
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");

            //Create An Instance Of The Stat Manager Passing In The Path Of The ZSNES Save State.            
            var sm = new StatManager(_saveStatePath);

            //Generate The Boxscore
            var boxscore = sm.GenerateBoxScore();

            Assert.True(boxscore!=null, "BoxScore Generated");
        }

        [Fact()]
        public void DisposeTest()
        {
            Boxscore boxscore;

            //Set The Path Of The ZSNES Save State File            
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");

            //Create An Instance Of The Stat Manager Passing In The Path Of The ZSNES Save State.            
            var sm = new StatManager(_saveStatePath);

            //Call Dispose
            sm.Dispose();

            //Calling The GenerateBoxScore Method should fail as the FileStream Reader should be closed.
            Exception ex = Assert.Throws<System.ArgumentException>(() => boxscore = sm.GenerateBoxScore());

            //Verify The Expected Exception was thrown.
            Assert.Equal("Stream was not readable.", ex.Message);
            
        }
    }
}