using System;
using System.IO;
using Nhl94StatsReader;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {

            
            //Set The Path Of The ZSNES Save State File
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");

            //Create An Instance Of The Stat Manager Passing In The Path Of The ZSNES Save State.
            //var sm = new StatManager(_saveStatePath);            
            var sm = new StatManager(_saveStatePath);

            //Generate The Boxscore
            var boxscore = sm.GenerateBoxScore();            

            //Dispose The StatManager
            sm.Dispose();

            //Set The StatManager Object = Null
            sm = null;

        }


    }
}
