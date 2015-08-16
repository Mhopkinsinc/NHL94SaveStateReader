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
    public class StatReaderTests
    {
        [Fact()]
        public void ValidateZsnesSaveStateTest()
        {

            //Set The Path Of The ZSNES Save State File            
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
                        
            var sr = new StatReader(_saveStatePath);            

            Assert.True(sr.ValidSaveStateFile, "Valid ZSNES File");
        }
    }
}