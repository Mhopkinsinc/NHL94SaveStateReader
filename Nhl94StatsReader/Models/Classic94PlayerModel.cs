using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhl94StatsReader
{
    
    public class Classic94PlayerModel : List<Classic94PlayerModel.Classic94Player>
    {
        public class Classic94Player
        {
            public int PlayerID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Team { get; set; }
            public string Position { get; set; }
            public int Jersey { get; set; }
            public int RosterID { get; set; }            
        }        
    }


}
