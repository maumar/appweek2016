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
        public string League { get; set; }

        public DateTime DateAdded { get; set; }

        public Account Account { get; set; }
        public string AccountName { get; set; }

        public List<Item> Items { get; set; }
    }
}
