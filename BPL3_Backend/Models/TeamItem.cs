using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend.Models
{
    public class TeamItem
    {
        public List<Item> Items { get; set; }
        public Team Team { get; set; }
    }
}
