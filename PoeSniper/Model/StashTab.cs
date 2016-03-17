using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StashTab
    {
        public string Id { get; set; }

        public string TabName { get; set; }

        public Stash Stash { get; set; }

        public List<Item> Items { get; set; }
    }
}
