using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItemIndexer
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
        public bool identified { get; set; }
        public bool corrupted { get; set; }
        public object requirements { get; set; }
        public List<object> implicitMods { get; set; }
        public List<object> explicitMods { get; set; }
        public string inventoryId { get; set; }
        public string note { get; set; }
    }
}
/*
 * 
 * 
 *  "verified": false,
  "w": 2,
  "h": 1,
  "ilvl": 70,
  "icon": "http://webcdn.pathofexile.com/image/Art/2DItems/Belts/Belt4.png?scale=1&w=2&h=1&v=da282d3a3d76fc0d14b882450c3ed2ae3",
  "support": true,
  "league": "Standard",
  "id": "8ded67a2cf095dec2e22d2c60db473b4214fd31152bcab28a11bace703996037",
  "sockets": [],
  "name": "",
  "typeLine": "Cloth Belt",
  "identified": true,
  "corrupted": false,
  "lockedToCharacter": false,
  "requirements": [
    {
      "name": "Level",
      "values": [
        [
          "16",
          0
        ]
      ],
      "displayMode": 0
    }
  ],
  "implicitMods": [
    "15% increased Stun Recovery"
  ],
  "frameType": 0,
  "x": 10,
  "y": 9,
  "inventoryId": "Stash1",
  "socketedItems": []
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */
