using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerModel
{
    public class IndexerItem
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public int Ilvl { get; set; }
        public bool Corrupted { get; set; }

        // FK -> Stash
        public string StashTabId { get; set; }
    }
}
