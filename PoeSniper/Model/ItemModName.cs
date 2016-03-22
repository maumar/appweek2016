using System.Collections.Generic;

namespace Model
{
    public class ItemModName
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public List<ItemMod> ItemMods { get; set; }
    }
}
