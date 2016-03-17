namespace Model
{
    public class ItemMod
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal? Value { get; set; }



        // FK -> Item (implicit mod)
        public string ItemImplicitId { get; set; }
        public Item ItemImplicit { get; set; }

        // FK -> Item (explicit mod)
        public string ItemExplicitId { get; set; }
        public ItemWithExplicitMods ItemExplicit { get; set; }
    }
}
