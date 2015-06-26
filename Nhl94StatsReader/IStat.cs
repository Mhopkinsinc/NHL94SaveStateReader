using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public interface IStat
    {
        long Offset { get; set; }
        string Statname { get; set; }
        StatType StatType { get; set; }      
        object StatValue { get; set; }      
    }
}
