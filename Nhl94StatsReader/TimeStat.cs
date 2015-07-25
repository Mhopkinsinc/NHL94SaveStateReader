using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class TimeStat : IStat
    {

        #region Public Variables

        IStatReader _statReader;

        public long[] Offsets
        { get; set; }

        public string Statname
        { get; set; }

        public StatType Stattype
        { get; set; }       

        #endregion

        #region Private Variables        

        private List<string> _offsetResults = new List<string>();

        private String _statValueHex { get; set; }

        private int _statValueInt { get; set; }

        #endregion

        public TimeStat(IStatReader StatReader)
        {
            this._statReader = StatReader;
        }
        
        public void ReadStat()
        {

            foreach (long offset in Offsets)
            {
               var result = _statReader.ReadStat(offset);
                _offsetResults.Add(result.ToString("X"));
                _statValueHex += result.ToString("X");
            }

            _statValueInt = Convert.ToInt32(_statValueHex, 16);
            var timespan = TimeSpan.FromSeconds(_statValueInt);

            _statReader = null;

            //LOG
            Console.WriteLine(Statname + " - Offset : " + Offsets[0] + " , Hex Value : " + _offsetResults[0]);
            Console.WriteLine(Statname + " - Offset : " + Offsets[1] + " , Hex Value : " + _offsetResults[1]);
            Console.WriteLine(Statname + " - Combined Hex Value : " + _statValueHex);
            Console.WriteLine(Statname + " - Combined Decimal Value : " + _statValueInt);
            Console.WriteLine(timespan.ToString(@"mm\:ss"));
            Console.WriteLine();

    
        }

       
        
        public void Log() { }
    }
}
