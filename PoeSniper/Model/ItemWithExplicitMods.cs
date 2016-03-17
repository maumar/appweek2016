using System.Collections.Generic;

namespace Model
{
    public class ItemWithExplicitMods : Item
    {
        public List<ItemMod> ExplicitMods { get; set; }

        public bool Identified { get; set; }
    }
}
