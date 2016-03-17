namespace IndexerModel
{
    public class IndexerItemMod
    {
        // composite key
        public string ItemId { get; set; }
        public int Index { get; set; }

        // FK -> Item (implicit mod)
        public string ItemImplicitId { get; set; }

        // FK -> Item (explicit mod)
        public string ItemExplicitId { get; set; }

        public decimal? Value { get; set; }

        // FK -> ModName
        public int ModNameId { get; set; }
    }
}
