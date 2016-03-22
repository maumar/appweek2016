using System.Collections.Generic;

namespace Model
{
    public class Item
    {
        // composite key
        public string Id { get; set; }
        public string Name { get; set; }

        public int Ilvl { get; set; }
        public bool Corrupted { get; set; }

        // FK -> Stash
        public string StashTabId { get; set; }
        public StashTab StashTab { get; set; }

        public List<ItemMod> ImplicitMods { get; set; }
    }
}
