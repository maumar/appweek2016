namespace Model
{
    public class FeedChunkAccount
    {
        public string ChunkId { get; set; }
        public int Index { get; set; }
        public FeedChunk Chunk { get; set; }

        public string AccountName { get; set; }
        public Account Account { get; set; }
    }
}
