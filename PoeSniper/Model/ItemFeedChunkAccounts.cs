namespace Model
{
    public class ItemFeedChunkAccounts
    {
        public string ChunkId { get; set; }
        public int Index { get; set; }
        public ItemFeedChunk Chunk { get; set; }

        public string AccountName { get; set; }
        public Account Account { get; set; }
    }
}
