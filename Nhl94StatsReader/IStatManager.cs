﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    public interface IStatManager
    {
        void AddStat(IStat stat);
        void RemoveStat();

    }
}