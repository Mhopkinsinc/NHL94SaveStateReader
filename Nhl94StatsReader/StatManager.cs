using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class StatManager :IStatManager
    {

        Stat[] _Stats;        

        public void AddStat(IStat stat)
        {
            throw new NotImplementedException();
        }

        public void RemoveStat()
        {
            throw new NotImplementedException();
        }
    }
}
