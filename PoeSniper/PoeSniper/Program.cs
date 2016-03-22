using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;
using Newtonsoft.Json;

namespace PoeSniper
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new PoeSniperContext())
            {
                while(true)
                {
                    Console.WriteLine(DateTime.Now + " Updating query data");
                    var rootObject = ReadItemQueriesJson();
                    foreach(var itemQuery in rootObject.itemQueries)
                    {
                        Console.Write(DateTime.Now + " Query: " + itemQuery.queryName);

                        IQueryable<Item> query = ctx.Items.Include(e => e.StashTab.Account);
                        if (itemQuery.itemName != null)
                        {
                            query = query.Where(e => e.Name == itemQuery.itemName);
                        }
                        if (itemQuery.itemNamePattern != null)
                        {
                            query = query.Where(e => e.Name.Contains(itemQuery.itemNamePattern));
                        }

                        if (itemQuery.properties != null)
                        {
                            var explicitModsQuery = query.OfType<ItemWithExplicitMods>();

                            foreach (var itemProperty in itemQuery.properties)
                            {
                                if (itemProperty.minValue == null && itemProperty.maxValue == null)
                                {
                                    explicitModsQuery = explicitModsQuery.Where(e => e.ExplicitMods.Any(m => m.ModName.Text == itemProperty.name));
                                }
                                else if (!string.IsNullOrEmpty(itemProperty.minValue) && string.IsNullOrEmpty(itemProperty.maxValue))
                                {
                                    var parsedMinValue = decimal.Parse(itemProperty.minValue);
                                    explicitModsQuery = explicitModsQuery.Where(e => e.ExplicitMods.Any
                                        (m => m.ModName.Text == itemProperty.name && m.Value >= parsedMinValue));
                                }
                                else if (!string.IsNullOrEmpty(itemProperty.maxValue) && string.IsNullOrEmpty(itemProperty.minValue))
                                {
                                    var parsedMaxValue = decimal.Parse(itemProperty.maxValue);
                                    explicitModsQuery = explicitModsQuery.Where(e => e.ExplicitMods.Any
                                        (m => m.ModName.Text == itemProperty.name && m.Value <= parsedMaxValue));
                                }
                                else if (!string.IsNullOrEmpty(itemProperty.maxValue) && !string.IsNullOrEmpty(itemProperty.minValue))
                                {
                                    var parsedMinValue = decimal.Parse(itemProperty.minValue);
                                    var parsedMaxValue = decimal.Parse(itemProperty.maxValue);
                                    explicitModsQuery = explicitModsQuery.Where(e => e.ExplicitMods.Any
                                        (m => m.ModName.Text == itemProperty.name
                                        && m.Value >= parsedMinValue
                                        && m.Value <= parsedMaxValue));
                                }
                            }

                            var resultss = explicitModsQuery.ToList();
                            
                            var results = resultss.Select(e =>
                            new
                            {
                                e.Name,
                                e.StashTab.Account.AccountName,
                                e.StashTab.TabName,
                                e.StashTab.Account.LastCharacterName
                            });

                            Console.WriteLine(" Found " + results.Count() + " match(es)");
                            if (results.Count() > 0)
                            {
                                foreach (var result in results)
                                {
                                    Console.WriteLine("Item name: " + result.Name + " Seller: " + result.AccountName + " Shop location: " + result.TabName);
                                }

                                Console.WriteLine("// TODO: make noise, send text, etc...");
                                Console.WriteLine();
                            }
                        }
                        else
                        {
                            var results = query.ToList().Select(e =>
                            new
                            {
                                e.Name,
                                e.StashTab.Account.AccountName,
                                e.StashTab.TabName,
                                e.StashTab.Account.LastCharacterName
                            });

                            Console.WriteLine(" Found " + results.Count() + " match(es)");
                            if (results.Count() > 0)
                            {
                                foreach (var result in results)
                                {
                                    Console.WriteLine("Item name: " + result.Name + " Seller: " + result.AccountName + " Shop location: " + result.TabName);
                                }

                                Console.WriteLine("// TODO: make noise, send text, etc...");
                                Console.WriteLine();
                            }
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine(DateTime.Now + " Sleeping for 30 sec");
                    Thread.Sleep(30000);
                    Console.WriteLine();
                }
            }
        }





        private static RootObject ReadItemQueriesJson()
        {
            var itemQueryText = File.ReadAllText("Item_Queries.json");
            var rootObject = JsonConvert.DeserializeObject<RootObject>(itemQueryText);

            return rootObject;
        }

    }

    public class JsonProperty
    {
        public string name { get; set; }
        public string minValue { get; set; }
        public string maxValue { get; set; }
    }

    public class JsonItemQuery
    {
        public string queryName { get; set; }
        public string itemName { get; set; }
        public string itemNamePattern { get; set; }
        public List<JsonProperty> properties { get; set; }
    }

    public class RootObject
    {
        public List<JsonItemQuery> itemQueries { get; set; }
    }

}
