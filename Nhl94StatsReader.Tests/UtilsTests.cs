using Xunit;
using Nhl94StatsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader.Tests
{
    public class UtilsTests
    {
        [Theory]
        [InlineData("CGY")]
        public void GetTeamRosterTest(string value)
        {

            var roster = Utils.GetTeamRoster(value);
            var player = roster.First();

            Assert.True(player.Team == value, "The Correct Roster Was Returned");
        }

        [Theory]
        [InlineData(3, "CGY")]
        public void GetTeamAbbreviationTest(int TeamID, string ExpectedTeamAbbreviation)
        {

            var TeaamAbbreviation = Utils.GetTeamAbbreviation(TeamID);

            Assert.True(TeaamAbbreviation == ExpectedTeamAbbreviation, "The Correct Team Was Returned");
        }

        [Theory]
        [InlineData("CGY", 1, "Jeff, Reese")]        
        public void GetPlayerTest(string TeamAbbreviation, int RosterId, string ExpectedPlayerName)
        {

            var playername = Utils.GetPlayer(TeamAbbreviation, RosterId);            

            Assert.True(playername.Contains(ExpectedPlayerName), "The Expected Player Was Returned");
        }
    }
}