using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
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

            //Load The ZSNES Save State File            
            sm.LoadSaveState(_saveStatePath);            

            //Load Default Stats
            sm.LoadDefaultStats();

            Console.ReadLine();            

            //Set The Box Score Values
            // boxscore.hometeam = hometeam.readstat;


        }


    }
}
