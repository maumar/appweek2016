using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model
{
    public class ItemFeedChunk
    {
        public string ChunkId { get; set; }
        public int Index { get; set; }

        public ItemFeedChunk NextChunk { get; set; }
    }
}
