using System.Collections.Generic;

namespace Model
{
    public class ItemFeedChunk
    {
        // composite key
        public string ChunkId { get; set; }
        public int Index { get; set; }

        public string NextChunkId { get; set; }

        public List<ItemFeedChunkAccounts> ChunkAccounts { get; set; }
    }
}
