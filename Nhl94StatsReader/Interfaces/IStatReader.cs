using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public interface IStatReader
    {
        byte ReadStat(long offset);
        int ReadlLittleEndian(long offset1, long offset2);
        void Close();
    }
}
