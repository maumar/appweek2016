using System.Collections.Generic;

namespace Model
{
    public class Stash
    {
        // composite key
        public string AccountName { get; set; }
        public string League { get; set; }


        public Account Account { get; set; }
        public List<StashTab> StashTabs { get; set; }
    }
}
