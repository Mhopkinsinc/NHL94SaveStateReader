using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class TimeStat
    {

        private FileStream _fileStream;
        private string _saveState;
        private List<string> _offsetValues = new List<string>();

        public long[] Offsets
        { get; set; }

        public string Statname
        { get; set; }

        public StatType Stattype
        { get; set; }                

        private int _statValueInt
        { get; set; }

        private String _statValueHex
        { get; set; }

        public void ReadStat()
        {
           _saveState = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SaveState\nhl94.zs3");
            
            using (_fileStream = File.OpenRead(_saveState))
            {
                using (BinaryReader w = new BinaryReader(_fileStream))
                {

                    foreach (long offset in Offsets)
                    {
                        _fileStream.Seek(offset, SeekOrigin.Begin);
                        var result = w.ReadByte();                        
                        _offsetValues.Add(result.ToString("X"));
                        Console.WriteLine(Statname + " : " + result.ToString("X")); //LOG
                        _statValueHex += result.ToString("X");                                                
                    }

                    _statValueInt = Convert.ToInt32(_statValueHex, 16);
                    var timespan = TimeSpan.FromSeconds(_statValueInt);
                    Console.WriteLine(timespan.ToString(@"mm\:ss")); //LOG

                }

            }

        }

        // Write To BoxScore
        public void WriteStat() { }
    }
}
