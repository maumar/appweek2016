using System.Collections.Generic;

namespace Model
{
    public class Account
    {
        public string AccountName { get; set; }

        public List<Stash> Stashes { get; set; }

        public string LastCharacterName { get; set; }

        public List<ItemFeedChunkAccounts> ChunkAccounts { get; set; }
    }
}
