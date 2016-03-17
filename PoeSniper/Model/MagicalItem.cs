using System.Collections.Generic;

namespace Model
{
    public class MagicalItem : Item
    {
        public List<ItemMod> ExplicitMods { get; set; }

        public bool Identified { get; set; }
    }
}
