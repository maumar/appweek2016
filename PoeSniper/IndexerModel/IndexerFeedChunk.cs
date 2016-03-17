using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerModel
{
    public class IndexerFeedChunk
    {
        // composite key
        public string ChunkId { get; set; }
        public int Index { get; set; }

        public string NextChunkId { get; set; }
    }
}
