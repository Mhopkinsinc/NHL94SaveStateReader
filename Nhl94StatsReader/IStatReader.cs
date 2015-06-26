using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public interface IStatReader
    {
        void ReadStat(IStat stat);
        void Write();
    }
}
