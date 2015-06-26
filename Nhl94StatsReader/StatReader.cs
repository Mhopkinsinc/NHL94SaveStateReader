using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class StatReader :IStatReader
    {
        
        private string _saveState;          // = @"C:\Users\Mark\Desktop\nhl94.zs3";
        private FileStream _fileStream;

        

        public StatReader(string SaveStatePath)
        {
            _saveState = SaveStatePath;
        }        

        
        public void ReadStat(IStat stat)
        {

            using (_fileStream = File.OpenRead(_saveState))
               {
                   using (BinaryReader w = new BinaryReader(_fileStream))
                   {

                    
                           _fileStream.Seek(stat.Offset, SeekOrigin.Begin);
                           var result = w.ReadByte();                       
                           Console.WriteLine(stat.Statname + " : " + result);                           
                           Console.ReadLine();
                      

                   }

               }
        }

        public void ReadStats(List<IStat> stats)
        {

            using (_fileStream = File.OpenRead(_saveState))
            {
                using (BinaryReader w = new BinaryReader(_fileStream))
                {

                    foreach (Stat stat in stats)
                    {
                    _fileStream.Seek(stat.Offset, SeekOrigin.Begin);
                    var result = w.ReadByte();
                    Console.WriteLine(stat.Statname + " : " + result);
                    stat.StatValue = result;
                    stat.StatValueHex = result.ToString("X");
                    //Console.ReadLine();
                    }

                }

            }


        }

        public void Write()
        {
            throw new NotImplementedException();
        }
    }
}
