using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class StatReader : IStatReader
    {
        public String SaveStatePath
        {get; set;}

        private FileStream _fileStream;
        private String _saveStatePath;          

        public StatReader()
        {
            if (_saveStatePath == null) _saveStatePath = SaveStatePath;
            _fileStream = File.OpenRead(_saveStatePath);
        }

        public byte ReadStat(long offset)
        {
            byte _result;

            using (BinaryReader w = new BinaryReader(_fileStream))
            {
                _fileStream.Seek(offset, SeekOrigin.Begin);
                _result = w.ReadByte();
            }

            return _result;

        }
    }
}
