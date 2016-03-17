using System.Collections.Generic;

namespace Indexer
{
    public class JsonStash
    {
        public string accountName { get; set; }
        public string lastCharacterName { get; set; }
        public string id { get; set; }
        public string stash { get; set; }
        public string stashType { get; set; }
        public List<JsonItem> items { get; set; }
        public bool @public { get; set; }
    }

    public class RootObject
    {
        public string next_change_id { get; set; }
        public List<JsonStash> stashes { get; set; }
    }

    public class JsonItem
    {
        public string id { get; set; }

        public int w { get; set; }
        public int h { get; set; }
        public int ilvl { get; set; }
        public List<object> sockets { get; set; }
        public string Name { get; set; }
        public string typeLine { get; set; }
        public string league { get; set; }
        public bool identified { get; set; }
        public bool corrupted { get; set; }
        public List<JsonRequirement> requirements { get; set; }
        public List<string> implicitMods { get; set; }
        public List<string> explicitMods { get; set; }
        public List<string> flavourText { get; set; }
        public string inventoryId { get; set; }
        public string note { get; set; }
    }

    public class JsonRequirement
    {
        public string name { get; set; }
        public List<List<object>> values { get; set; }
    }
}
