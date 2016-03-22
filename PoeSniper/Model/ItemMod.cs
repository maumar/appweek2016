namespace Model
{
    public class ItemMod
    {
        // composite key
        public string ItemId { get; set; }
        public int Index { get; set; }

        // FK -> Item (implicit mod)
        public decimal? Value { get; set; }

        // FK -> Item (implicit mod)
        public string ItemImplicitId { get; set; }
        public Item ItemImplicit { get; set; }

        // FK -> Item (explicit mod)
        public string ItemExplicitId { get; set; }
        public ItemWithExplicitMods ItemExplicit { get; set; }

        // FK -> ModName
        public int ModNameId { get; set; }
        public ItemModName ModName { get; set; }
    }
}
