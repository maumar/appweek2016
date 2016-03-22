using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using IndexerModel;
using Model;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var nextChunkId = "";
            int index = 0;
            var itemModNameMapping = new Dictionary<string, int>();
            using (var ctx = new PoeSniperContext())
            {
                //ctx.Database.EnsureDeleted();
                //ctx.Database.EnsureCreated();

                var lastChunk = ctx.FeedChunks.OrderByDescending(e => e.Index).FirstOrDefault();
                nextChunkId = lastChunk != null ? lastChunk.NextChunkId : "";
                index = lastChunk?.Index ?? 0;

                itemModNameMapping = ctx.ItemModNames.ToDictionary(e => e.Text, e => e.Id);
            }

            //nextChunkId = "1844560-1954128-1822803-2139656-2027865";
            var cleanupTimer = 0;

            while (true)
            {
                nextChunkId = FetchItemFeedChunk(nextChunkId, index, itemModNameMapping);
                index++;
                cleanupTimer++;
                if (cleanupTimer == 10)
                {
                    //DeleteStaleStashTabs();
                    cleanupTimer = 0;
                }
            }
        }

        private static void DeleteStaleStashTabs()
        {
            var timeStamp = DateTime.Now.AddMinutes(-60);

            using (var ctx = new PoeSniperContext())
            {
                Console.WriteLine("Removing stale stash tabs");

                Console.WriteLine("items before: " + ctx.Items.Count());
                var sw = new Stopwatch();
                sw.Start();
                var result = ctx.Database.ExecuteSqlCommand("DELETE FROM [StashTabs] WHERE DateAdded < {0}", timeStamp);

                Console.WriteLine("Done. Removed " + result + " tabs. " + sw.Elapsed);
                Console.WriteLine("items after: " + ctx.Items.Count());
                Console.WriteLine();
            }
        }

        private static RootObject GetJsonRootObject(string chunkId)
        {
            var apiUrl = "http://www.pathofexile.com/api/public-stash-tabs/" + chunkId;

            Console.WriteLine(DateTime.Now + " Fetching " +
                (string.IsNullOrEmpty(chunkId)
                    ? "first chunk"
                    : "chunk with ID: " + chunkId) + " ");

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(handler);
            var getStreamTask = httpClient.GetStreamAsync(apiUrl);
            getStreamTask.Wait();
            var stream = getStreamTask.Result;

            string value = string.Empty;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                value = reader.ReadToEnd();
            }

            var rootObject = JsonConvert.DeserializeObject<RootObject>(value);

            return rootObject;
        }

        private static List<IndexerAccount> CreateAccounts(IndexerPoeSniperContext ctx, string chunkId, int lastChunkIndex, RootObject rootObject)
        {
            var feedChunk = new IndexerFeedChunk
            {
                ChunkId = chunkId,
                Index = lastChunkIndex + 1,
                NextChunkId = rootObject.next_change_id,
            };

            ctx.FeedChunks.Add(feedChunk);
            ctx.SaveChanges();

            var accountNames = rootObject.stashes.Select(e => e.accountName).Distinct();
            var existingAccounts = ctx.Accounts.Where(e => accountNames.Contains(e.AccountName)).ToList();

            var newAccountsJson = rootObject.stashes
                .Select(e => new { e.accountName, e.lastCharacterName })
                .Distinct()
                .Where(e => !existingAccounts.Select(ea => ea.AccountName).Contains(e.accountName));

            var newAccounts = new List<IndexerAccount>();
            var feedChunkAccounts = new List<IndexerFeedChunkAccount>();
            foreach (var newAccountJson in newAccountsJson.Where(e => e.accountName != null))
            {
                var newAccount = new IndexerAccount
                {
                    AccountName = newAccountJson.accountName,
                    LastCharacterName = newAccountJson.lastCharacterName,
                };

                newAccounts.Add(newAccount);
            }

            var allAccounts = existingAccounts.Concat(newAccounts).ToList();
            foreach (var account in allAccounts)
            {
                var feedChunkAccount = new IndexerFeedChunkAccount
                {
                    ChunkId = feedChunk.ChunkId,
                    Index = feedChunk.Index,
                    AccountName = account.AccountName,
                };

                feedChunkAccounts.Add(feedChunkAccount);
            }

            ctx.Accounts.AddRange(newAccounts);
            ctx.SaveChanges();

            ctx.FeedChunkAccounts.AddRange(feedChunkAccounts);
            ctx.SaveChanges();

            return allAccounts;
        }

        public static string FetchItemFeedChunk(string chunkId, int lastChunkIndex, Dictionary<string, int> itemModNameMapping)
        {
            var rootObject = GetJsonRootObject(chunkId);

            Console.WriteLine("Processing");
            var sw = new Stopwatch();
            sw.Start();

            var stashTabs = new List<IndexerStashTab>();
            var items = new List<IndexerItem>();
            var itemMods = new List<IndexerItemMod>();
            var newItemModNames = new List<IndexerItemModName>();
            using (var ctx = new IndexerPoeSniperContext())
            {
                var accounts = CreateAccounts(ctx, chunkId, lastChunkIndex, rootObject);

                var stashTabIds = rootObject.stashes.Select(s => s.id).ToArray();
                var exisitingStashTabs = ctx.StashTabs.Where(st => stashTabIds.Contains(st.Id)).ToList();
                if (exisitingStashTabs.Count > 0)
                {
                    Console.WriteLine("Removing existing stash tabs: " + exisitingStashTabs.Count);

                    ctx.StashTabs.RemoveRange(exisitingStashTabs);
                    ctx.SaveChanges();
                }

                var itemCount = 0;

                var damageRangeRegex = new Regex(@"(?<damageRange>\d+-\d+)", RegexOptions.Compiled);
                var genericPropertyRegex = new Regex(@"(?<value>" + Regex.Escape("+") + @"?-?\d+" + Regex.Escape(".") + @"?\d*)", RegexOptions.Compiled);


                var i = 0;
                var timeStamp = DateTime.Now;
                foreach (var jsonStash in rootObject.stashes.Where(e => e.accountName != null))
                {
                    i++;

                    var account = accounts.Where(a => a.AccountName == jsonStash.accountName).Single();

                    if (jsonStash.accountName == null)
                    {
                        continue;
                    }

                    var league = jsonStash.items.FirstOrDefault()?.league ?? "Unknown";
                    if (league != "Hardcore Perandus")
                    {
                        continue;
                    }

                    var stashTab = new IndexerStashTab
                    {
                        Id = jsonStash.id,
                        TabName = jsonStash.stash,
                        DateAdded = timeStamp,
                        AccountName = jsonStash.accountName,
                    };

                    stashTabs.Add(stashTab);

                    foreach (var jsonItem in jsonStash.items)
                    {
                        itemCount++;

                        var modIndex = 0;
                        if (jsonItem.explicitMods != null)
                        {
                            foreach (var mod in jsonItem.explicitMods)
                            {
                                modIndex++;

                                if (mod.StartsWith("<"))
                                {
                                    continue;
                                }

                                decimal modValue;
                                string modName;

                                var match = damageRangeRegex.Match(mod);
                                if (match.Success)
                                {
                                    modName = mod.Replace(match.Value, "X");
                                    var range = match.Value.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                                    modValue = (decimal.Parse(range[0]) + decimal.Parse(range[1])) / 2.0M;
                                }
                                else
                                {
                                    match = genericPropertyRegex.Match(mod);
                                    if (decimal.TryParse(match.Value, out modValue))
                                    {
                                        modName = mod.Replace(match.Value, "X");
                                    }
                                    else
                                    {
                                        modName = mod;
                                    }
                                }

                                int modNameId;
                                if (!itemModNameMapping.TryGetValue(modName, out modNameId))
                                {
                                    var newItemModName = new IndexerItemModName
                                    {
                                        Id = itemModNameMapping.Count + 1,
                                        Text = modName,
                                    };

                                    itemModNameMapping.Add(newItemModName.Text, newItemModName.Id);
                                    newItemModNames.Add(newItemModName);
                                    modNameId = newItemModName.Id;
                                }

                                var itemMod = new IndexerItemMod
                                {
                                    ItemId = jsonItem.id,
                                    Index = modIndex,
                                    ModNameId = modNameId,
                                    Value = modValue,
                                    ItemExplicitId = jsonItem.id,
                                };

                                itemMods.Add(itemMod);
                            }
                        }

                        var lastIndexOf = jsonItem.Name.LastIndexOf(">");
                        var itemName = lastIndexOf > -1 ? jsonItem.Name.Substring(lastIndexOf + 1) : jsonItem.Name;

                        if (itemName == "")
                        {
                            itemName = jsonItem.typeLine;
                        }
                        else
                        {
                            itemName = itemName + " " + jsonItem.typeLine;
                        }

                        IndexerItem item;
                        if (jsonItem.explicitMods == null)
                        {
                            item = new IndexerItem
                            {
                                Id = jsonItem.id,

                                Corrupted = jsonItem.corrupted,
                                Ilvl = jsonItem.ilvl,
                                Name = itemName,
                                StashTabId = jsonStash.id,
                            };
                        }
                        else
                        {
                            if (jsonItem.flavourText == null)
                            {
                                item = new IndexerItemWithExplicitMods
                                {
                                    Id = jsonItem.id,

                                    Corrupted = jsonItem.corrupted,
                                    Ilvl = jsonItem.ilvl,
                                    Name = itemName,
                                    StashTabId = jsonStash.id,
                                };
                            }
                            else
                            {
                                item = new IndexerUniqueItem
                                {
                                    Id = jsonItem.id,

                                    Corrupted = jsonItem.corrupted,
                                    Ilvl = jsonItem.ilvl,
                                    Name = itemName,
                                    StashTabId = jsonStash.id,
                                };
                            }
                        }

                        items.Add(item);
                    }
                }

                Console.WriteLine("Done " + sw.Elapsed);
            }

            sw.Restart();

            Console.Write("Saving to database: ");
            using (var ctx = new IndexerPoeSniperContext())
            {
                Console.Write("Tabs(" + stashTabs.Count + ")");
                ctx.StashTabs.AddRange(stashTabs);
                ctx.SaveChanges();

                Console.Write(" ModNames(" + newItemModNames.Count + ")");
                ctx.ItemModNames.AddRange(newItemModNames);
                ctx.SaveChanges();
            }

            Console.Write(" Items(" + items.Count + ")");
            IEnumerable<IndexerItem> remainingItems = items;
            while (remainingItems.Count() > 0)
            {
                var batchItems = remainingItems.Take(1000);
                remainingItems = remainingItems.Skip(1000);

                using (var ctx = new IndexerPoeSniperContext())
                {
                    ctx.Items.AddRange(batchItems);
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!!! ERROR WHEN SAVING ITEMS: " + ex.Message);
                    }
                }

                Console.Write(".");
            }


            Console.Write(" Mods(" + itemMods.Count + ")");
            IEnumerable<IndexerItemMod> remainingItemMods = itemMods;
            while (remainingItemMods.Count() > 0)
            {
                var batchItemMods = remainingItemMods.Take(1000);
                remainingItemMods = remainingItemMods.Skip(1000);

                using (var ctx = new IndexerPoeSniperContext())
                {
                    ctx.ItemMods.AddRange(batchItemMods);
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!!! ERROR WHEN SAVING ITEM MODS: " + ex.Message);
                    }
                }

                Console.Write(".");
            }

            Console.WriteLine(" Done " + sw.Elapsed);
            Console.WriteLine();

            return rootObject.next_change_id;
        }
    }
}
