using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class IntegerStat : IStat
    {

        #region Properties

        IStatReader _statReader;

        public long Offset
        { get; set; }

        public string Statname
        { get; set; }

        public StatType Stattype
        { get; set; }
        
        private List<string> _offsetResults = new List<string>();

        public String _statValueHex { get; set; }

        public int _statValueInt { get; set; }

        #endregion

        #region Constructors

        public IntegerStat(IStatReader StatReader)
        {
            this._statReader = StatReader;
        } 

        #endregion

        #region Methods

            public void ReadStat()
            {


                var result = _statReader.ReadStat(Offset);
                _offsetResults.Add(result.ToString("X"));
                _statValueInt = result;

                _statReader = null;

                //LOG
                Console.WriteLine(Statname + " - Offset : " + Offset + " , Hex Value : " + _offsetResults[0] + " , Int Value : " + _statValueInt);
                Console.WriteLine();
            }


            public void Log() { }

        #endregion
    }
}
