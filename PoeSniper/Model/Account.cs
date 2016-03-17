using System.Collections.Generic;

namespace Model
{
    public class Account
    {
        public string AccountName { get; set; }

        public string LastCharacterName { get; set; }

        public List<StashTab> StashTabs { get; set; }
        public List<FeedChunkAccount> ChunkAccounts { get; set; }
    }
}
