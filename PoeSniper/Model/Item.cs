using System.Collections.Generic;

namespace Model
{
    public class Item
    {
        // composite key
        public string Id { get; set; }
        public string Name { get; set; }

        public int Ilvl { get; set; }
        public string TypeLine { get; set; }
        public bool Corrupted { get; set; }

        public List<ItemRequirement> Requirements { get; set; }

        public List<ItemMod> ImplicitMods { get; set; }
    }
}
