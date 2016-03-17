using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerModel
{
    public class IndexerFeedChunkAccount
    {
        // FK -> Chunk
        public string ChunkId { get; set; }
        public int Index { get; set; }

        // FK -> Account
        public string AccountName { get; set; }
    }
}
