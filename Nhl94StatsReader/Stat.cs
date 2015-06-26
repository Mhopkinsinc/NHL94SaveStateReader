using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public class Stat : IStat
    {

        public long Offset
        { get; set; }

        public string Statname
        { get; set; }

        public StatType StatType
        { get; set; }

        public object StatValue
        { get; set; }

        public object StatValueHex
        { get; set; }
    }
}
