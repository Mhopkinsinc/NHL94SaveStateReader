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

        #region Private Variables

        public IStatReader _statreader;        
        
        private List<string> _offsetValues = new List<string>();

        private String _statValueHex { get; set; }

        #endregion

        #region Public Variables

        public long[] Offsets
        { get; set; }

        public string Statname
        { get; set; }

        public StatType Stattype
        { get; set; }

        private int _statValueInt
        { get; set; }

        #endregion

        public void ReadStat()
        {

            foreach (long offset in Offsets)
            {
               var result = _statreader.ReadStat(offset);
                _offsetValues.Add(result.ToString("X"));
                _statValueHex += result.ToString("X");
            }

                    _statValueInt = Convert.ToInt32(_statValueHex, 16);
                    var timespan = TimeSpan.FromSeconds(_statValueInt);

                    //LOG
                    Console.WriteLine(Statname + " - Offset : " + Offsets[0] + " , Hex Value : " + _offsetValues[0]);
                    Console.WriteLine(Statname + " - Offset : " + Offsets[1] + " , Hex Value : " + _offsetValues[1]);
                    Console.WriteLine(Statname + " - Combined Hex Value : " + _statValueHex);
                    Console.WriteLine(Statname + " - Combined Decimal Value : " + _statValueInt);
                    Console.WriteLine(timespan.ToString(@"mm\:ss"));

    
        }

        //public void ReadStat()
        //{   
        //    using (_fileStream = File.OpenRead(SaveStatePath))
        //    {
        //        using (BinaryReader w = new BinaryReader(_fileStream))
        //        {

        //            foreach (long offset in Offsets)
        //            {
        //                _fileStream.Seek(offset, SeekOrigin.Begin);
        //                var result = w.ReadByte();
        //                _offsetValues.Add(result.ToString("X"));
        //                _statValueHex += result.ToString("X");                                                
        //            }                    

        //            _statValueInt = Convert.ToInt32(_statValueHex, 16);
        //            var timespan = TimeSpan.FromSeconds(_statValueInt);

        //            //LOG
        //            Console.WriteLine(Statname + " - Offset : " + Offsets[0] + " , Hex Value : " + _offsetValues[0]); 
        //            Console.WriteLine(Statname + " - Offset : " + Offsets[1] + " , Hex Value : " + _offsetValues[1]); 
        //            Console.WriteLine(Statname + " - Combined Hex Value : " + _statValueHex); 
        //            Console.WriteLine(Statname + " - Combined Decimal Value : " + _statValueInt); 
        //            Console.WriteLine(timespan.ToString(@"mm\:ss")); 

        //        }

        //    }

        //}     


        // Write To BoxScore
        
        public void Log() { }
    }
}
