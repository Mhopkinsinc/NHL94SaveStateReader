using System;
using System.IO;
using Nhl94StatsReader;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {

            //Create An Instance Of The Stat Manager
            var sm = new StatManager();            
            
            //Set The Path Of The ZSNES Save State File
            var _saveStatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");            

            //Load The Save State File            
            sm.LoadSaveState(_saveStatePath);

            //Generate The Boxscore
            var boxscore = sm.GenerateBoxScore();

            //Dispose The StatManager
            sm.Dispose();

            sm = null;


        }


    }
}
