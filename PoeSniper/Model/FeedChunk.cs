using System.Collections.Generic;

namespace Model
{
    public class FeedChunk
    {
        // composite key
        public string ChunkId { get; set; }
        public int Index { get; set; }

        public string NextChunkId { get; set; }

        public List<FeedChunkAccount> FeedChunkAccounts { get; set; }
    }
}
