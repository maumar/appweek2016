using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Model;
using Newtonsoft.Json;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new PoeSniperContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                var lastChunk = ctx.ItemFeedChunks.OrderByDescending(e => e.Index).FirstOrDefault();
                var nextChunkId = lastChunk != null ? lastChunk.NextChunkId : "";
                int index = lastChunk?.Index ?? 0;

                var query = ctx.ItemFeedChunks
                    .Include(c => c.ChunkAccounts)
                    .ThenInclude(ca => ca.Account)
                    .ThenInclude(a => a.Stashes)
                    .ThenInclude(s => s.StashTabs)
                    .ThenInclude(s => s.Items)
                    .Where(c => c.ChunkAccounts.Any(ca => ca.Account.Stashes.Count > 0)).Take(2);


                var result = query.ToList();



                nextChunkId = "1618738-1717914-1600548-1876913-1779100";

                while (true)
                {
                    nextChunkId = FetchItemFeedChunk(nextChunkId, index);
                    index++;
                }
            }
        }

        public static string FetchItemFeedChunk(string chunkId, int lastChunkIndex)
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

            var sw = new Stopwatch();
            sw.Start();




            ItemFeedChunk newChunk;
            using (var ctx = new PoeSniperContext())
            {
                newChunk = new ItemFeedChunk
                {
                    ChunkId = chunkId,
                    Index = lastChunkIndex + 1,
                    NextChunkId = rootObject.next_change_id,
                    ChunkAccounts = new List<ItemFeedChunkAccounts>(),
                };

                ctx.ItemFeedChunks.Add(newChunk);
                ctx.SaveChanges();

                var accountNames = rootObject.stashes.Select(s => s.accountName).Distinct().ToArray();
                var accounts = ctx.Accounts.Include(a => a.Stashes).ThenInclude<Account, Stash, List<StashTab>>(s => s.StashTabs)
                    .Where(e => accountNames.Contains(e.AccountName)).ToList();

                var stashIds = rootObject.stashes.Select(s => s.id).ToArray();
                var stashTabs = ctx.StashTabs.Where(st => stashIds.Contains(st.Id)).ToList();

                var chunkAccounts = new List<ItemFeedChunkAccounts>();
                foreach (var account in accounts)
                {
                    var chunkAccount = new ItemFeedChunkAccounts
                    {
                        ChunkId = newChunk.ChunkId,
                        Index = newChunk.Index,
                        AccountName = account.AccountName,
                    };

                    chunkAccounts.Add(chunkAccount);
                }
            }


            IEnumerable<JsonStash> stashes = rootObject.stashes;

            while (stashes.Any())
            {
                Fooooo(newChunk.ChunkId, newChunk.Index, stashes.Take(20), rootObject.stashes);
                stashes = stashes.Skip(20);
            }


            Console.WriteLine(sw.Elapsed);

                //using (var ctx = new PoeSniperContext())
                //{
                //    var newChunk = new ItemFeedChunk
                //    {
                //        ChunkId = chunkId,
                //        Index = lastChunkIndex + 1,
                //        NextChunkId = rootObject.next_change_id,
                //        ChunkAccounts = new List<ItemFeedChunkAccounts>(),
                //    };

                //    ctx.ItemFeedChunks.Add(newChunk);
                //    ctx.SaveChanges();

                //    Console.Write(
                //        (string.IsNullOrEmpty(chunkId)
                //            ? "First chunk"
                //            : "Chunk with ID " + chunkId));

                //    Console.WriteLine(": " + rootObject.stashes.Count + " stashes, " + rootObject.stashes.SelectMany(s => s.items).Count() + " items ");

                //    var accountNames = rootObject.stashes.Select(s => s.accountName).Distinct().ToArray();
                //    var accounts = ctx.Accounts.Include(a => a.Stashes).ThenInclude<Account, Stash, List<StashTab>>(s => s.StashTabs)
                //        .Where(e => accountNames.Contains(e.AccountName)).ToList();

                //    var stashIds = rootObject.stashes.Select(s => s.id).ToArray();
                //    var stashes = ctx.StashTabs.Where(st => stashIds.Contains(st.Id)).ToList();

                //    var chunkAccounts = new List<ItemFeedChunkAccounts>();
                //    foreach (var account in accounts)
                //    {
                //        var chunkAccount = new ItemFeedChunkAccounts
                //        {
                //            Chunk = newChunk,
                //            Account = account,
                //        };

                //        chunkAccounts.Add(chunkAccount);
                //    }

                //    var standardStashesCount = 0;
                //    var hardcoreStashesCount = 0;
                //    var emptyStashesCount = 0;
                //    var newAccountsCount = 0;
                //    var addedItemsCount = 0;
                //    var newStashTabs = new List<StashTab>();


                //    var damageRangeRegex = new Regex(@"(?<damageRange>\d+-\d+)", RegexOptions.Compiled);
                //    var genericPropertyRegex = new Regex(@"(?<value>" + Regex.Escape("+") + @"?-?\d+" + Regex.Escape(".") + @"?\d*)", RegexOptions.Compiled);

                //    var m = 0;

                //    var sta = 0;

                //    var itemCount = 0;

                //    var alltems = new List<Item>();
                //    var allItemMods = new List<ItemMod>();
                //    foreach (var jsonStash in rootObject.stashes)
                //    {
                //        sta++;
                //        var account = accounts.Where(a => a.AccountName == jsonStash.accountName).FirstOrDefault();

                //        if (account == null)
                //        {
                //            if (jsonStash.accountName == null)
                //            {
                //                continue;
                //            }

                //            account = new Account
                //            {
                //                AccountName = jsonStash.accountName,
                //                LastCharacterName = jsonStash.lastCharacterName,
                //                Stashes = new List<Stash>(),
                //            };

                //            ctx.Accounts.Add(account);
                //            accounts.Add(account);
                //            newAccountsCount++;

                //            var chunkAccount = new ItemFeedChunkAccounts
                //            {
                //                Chunk = newChunk,
                //                Account = account,
                //            };

                //            chunkAccounts.Add(chunkAccount);

                //            //ctx.SaveChanges();
                //        }

                //        var league = jsonStash.items.FirstOrDefault()?.league ?? "Unknown";

                //        if (league == "Standard")
                //        {
                //            standardStashesCount++;
                //            continue;
                //        }

                //        if (league == "Hardcore")
                //        {
                //            hardcoreStashesCount++;
                //            continue;
                //        }

                //        if (league == "Unknown")
                //        {
                //            emptyStashesCount++;
                //            continue;
                //        }


                //        var stash = account.Stashes
                //            .Where(s => s.League == league).FirstOrDefault();

                //        if (stash == null)
                //        {
                //            stash = new Stash
                //            {
                //                AccountName = account.AccountName,
                //                League = league,
                //                StashTabs = new List<StashTab>(),
                //            };

                //            ctx.Stashes.Add(stash);
                //            account.Stashes.Add(stash);
                //            //ctx.SaveChanges();
                //        }

                //        var stashTab = stash.StashTabs
                //            .Where(st => st.Id == jsonStash.id)
                //            .FirstOrDefault();

                //        if (stashTab != null)
                //        {
                //            // delete the stash tab - we want to clear all the items
                //            ctx.StashTabs.Remove(stashTab);
                //            stash.StashTabs.Remove(stashTab);

                //            Console.WriteLine("!!! Deleting stash tab");
                //            ctx.SaveChanges();
                //        }

                //        stashTab = new StashTab
                //        {
                //            Id = jsonStash.id,
                //            Stash = stash,
                //            Items = new List<Item>(),
                //            TabName = jsonStash.stash,
                //        };

                //        ctx.StashTabs.Add(stashTab);
                //        stash.StashTabs.Add(stashTab);
                //        newStashTabs.Add(stashTab);
                //        //ctx.SaveChanges();


                //        foreach (var jsonItem in jsonStash.items)
                //        {
                //            itemCount++;

                //            if (itemCount % 50 == 0)
                //            {
                //                Console.Write(" . ");
                //            }

                //            Item item;
                //            var itemMods = new List<ItemMod>();

                //            if (jsonItem.explicitMods != null)
                //            {
                //                foreach (var mod in jsonItem.explicitMods)
                //                {
                //                    if (mod.StartsWith("<"))
                //                    {
                //                        continue;
                //                    }

                //                    m++;

                //                    if (m % 250 == 0)
                //                    {
                //                        Console.Write("{" + itemCount + "}");
                //                    }

                //                    decimal modValue;
                //                    string modName;

                //                    var match = damageRangeRegex.Match(mod);
                //                    if (match.Success)
                //                    {
                //                        modName = mod.Replace(match.Value, "X");
                //                        var range = match.Value.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                //                        modValue = (decimal.Parse(range[0]) + decimal.Parse(range[1])) / 2.0M;
                //                    }
                //                    else
                //                    {
                //                        match = genericPropertyRegex.Match(mod);
                //                        if (decimal.TryParse(match.Value, out modValue))
                //                        {
                //                            modName = mod.Replace(match.Value, "X");
                //                        }
                //                        else
                //                        {
                //                            modName = mod;
                //                        }
                //                    }

                //                    var itemMod = new ItemMod
                //                    {
                //                        Name = mod.GetHashCode().ToString(),
                //                        Value = modValue,
                //                    };

                //                    itemMods.Add(itemMod);
                //                }
                //            }

                //            if (jsonItem.explicitMods == null && jsonItem.flavourText == null)
                //            {
                //                item = new Item
                //                {
                //                    Id = jsonItem.id,
                //                    Ilvl = jsonItem.ilvl,
                //                    Name = jsonItem.Name,
                //                    TypeLine = jsonItem.typeLine,
                //                    Corrupted = jsonItem.corrupted,
                //                };
                //            }
                //            else if (jsonItem.flavourText == null)
                //            {
                //                item = new MagicalItem
                //                {
                //                    Id = jsonItem.id,
                //                    Ilvl = jsonItem.ilvl,
                //                    Name = jsonItem.Name,
                //                    TypeLine = jsonItem.typeLine,
                //                    Corrupted = jsonItem.corrupted,
                //                    ExplicitMods = itemMods,
                //                };

                //                allItemMods.AddRange(itemMods);
                //                //ctx.ItemMods.AddRange(itemMods);
                //            }
                //            else
                //            {
                //                item = new UniqueItem
                //                {
                //                    Id = jsonItem.id,
                //                    Ilvl = jsonItem.ilvl,
                //                    Name = jsonItem.Name,
                //                    TypeLine = jsonItem.typeLine,
                //                    Corrupted = jsonItem.corrupted,
                //                    ExplicitMods = itemMods,
                //                };

                //                allItemMods.AddRange(itemMods);
                //                //ctx.ItemMods.AddRange(itemMods);
                //            }

                //            stashTab.Items.Add(item);
                //            alltems.Add(item);
                //            //ctx.Items.Add(item);
                //            addedItemsCount++;
                //        }
                //    }

                //    Console.WriteLine("Accounts - Exisitng: " + accounts.Count + " Added: " + newAccountsCount 
                //        + " | Stashes - 'Std': " + standardStashesCount + " HC: " + hardcoreStashesCount + " Empty: " + emptyStashesCount + " Added: " + newStashTabs.Count
                //        + " | Items - Added: " + addedItemsCount);
                //    Console.WriteLine();

                //    Console.Write("Adding items " + alltems.Count);
                //    ctx.Items.AddRange(alltems);
                //    Console.WriteLine("Done");

                //    Console.Write("Adding mods " + allItemMods.Count);
                //    ctx.ItemMods.AddRange(allItemMods);
                //    Console.WriteLine("Done");

                //    Console.Write("Adding accounts " + chunkAccounts.Count);
                //    ctx.ChunkAccounts.AddRange(chunkAccounts);
                //    Console.WriteLine("Done");

                //    Console.Write("Save changes ");
                //    ctx.SaveChanges();
                //    Console.WriteLine("Done");

                //}

                Console.WriteLine(sw.Elapsed);


            return rootObject.next_change_id;
        }


        private static  void Fooooo(string chunkId, int chunkIndex/*ItemFeedChunk newChunk*/, IEnumerable<JsonStash> jsonStashes, IEnumerable<JsonStash> allStashes)
        {
            using (var ctx = new PoeSniperContext())
            {

                var accountNames = jsonStashes.Select(s => s.accountName).Distinct().ToArray();
                var allAccountNames = allStashes.Select(s => s.accountName).Distinct().ToArray();
                var accounts = ctx.Accounts.Include(a => a.Stashes).ThenInclude<Account, Stash, List<StashTab>>(s => s.StashTabs)
                    .Where(e => accountNames.Contains(e.AccountName)).ToList();

                var allAccounts = ctx.Accounts.Include(a => a.Stashes).ThenInclude<Account, Stash, List<StashTab>>(s => s.StashTabs)
                    .Where(e => allAccountNames.Contains(e.AccountName)).ToList();

                var stashIds = jsonStashes.Select(s => s.id).ToArray();
                var stashes = ctx.StashTabs.Where(st => stashIds.Contains(st.Id)).ToList();





                var standardStashesCount = 0;
                var hardcoreStashesCount = 0;
                var emptyStashesCount = 0;
                var newAccountsCount = 0;
                var addedItemsCount = 0;
                var newStashTabs = new List<StashTab>();


                var damageRangeRegex = new Regex(@"(?<damageRange>\d+-\d+)", RegexOptions.Compiled);
                var genericPropertyRegex = new Regex(@"(?<value>" + Regex.Escape("+") + @"?-?\d+" + Regex.Escape(".") + @"?\d*)", RegexOptions.Compiled);

                var m = 0;

                var sta = 0;

                var itemCount = 0;

                var alltems = new List<Item>();
                var allItemMods = new List<ItemMod>();
                foreach (var jsonStash in jsonStashes)
                {
                    sta++;
                    var account = accounts.Where(a => a.AccountName == jsonStash.accountName).FirstOrDefault();

                    if (account == null)
                    {
                        if (jsonStash.accountName == null)
                        {
                            continue;
                        }

                        account = new Account
                        {
                            AccountName = jsonStash.accountName,
                            LastCharacterName = jsonStash.lastCharacterName,
                            Stashes = new List<Stash>(),
                        };

                        ctx.Accounts.Add(account);
                        accounts.Add(account);
                        newAccountsCount++;

                        var chunkAccount = new ItemFeedChunkAccounts
                        {
                            ChunkId = chunkId,
                            Index = chunkIndex,
                            AccountName = account.AccountName,
                        };

                        ctx.ChunkAccounts.Add(chunkAccount);
                        //chunkAccounts.Add(chunkAccount);

                        //ctx.SaveChanges();
                    }

                    var league = jsonStash.items.FirstOrDefault()?.league ?? "Unknown";

                    if (league == "Standard")
                    {
                        standardStashesCount++;
                        continue;
                    }

                    if (league == "Hardcore")
                    {
                        hardcoreStashesCount++;
                        continue;
                    }

                    if (league == "Unknown")
                    {
                        emptyStashesCount++;
                        continue;
                    }


                    var stash = account.Stashes
                        .Where(s => s.League == league).FirstOrDefault();

                    if (stash == null)
                    {
                        stash = new Stash
                        {
                            AccountName = account.AccountName,
                            League = league,
                            StashTabs = new List<StashTab>(),
                        };

                        ctx.Stashes.Add(stash);
                        account.Stashes.Add(stash);
                        //ctx.SaveChanges();
                    }

                    var stashTab = stash.StashTabs
                        .Where(st => st.Id == jsonStash.id)
                        .FirstOrDefault();

                    if (stashTab != null)
                    {
                        // delete the stash tab - we want to clear all the items
                        ctx.StashTabs.Remove(stashTab);
                        stash.StashTabs.Remove(stashTab);

                        Console.WriteLine("!!! Deleting stash tab");
                        ctx.SaveChanges();
                    }

                    stashTab = new StashTab
                    {
                        Id = jsonStash.id,
                        Stash = stash,
                        Items = new List<Item>(),
                        TabName = jsonStash.stash,
                    };

                    ctx.StashTabs.Add(stashTab);
                    stash.StashTabs.Add(stashTab);
                    newStashTabs.Add(stashTab);
                    //ctx.SaveChanges();


                    foreach (var jsonItem in jsonStash.items)
                    {
                        itemCount++;

                        if (itemCount % 50 == 0)
                        {
                            Console.Write(" . ");
                        }

                        Item item;
                        var itemMods = new List<ItemMod>();

                        if (jsonItem.explicitMods != null)
                        {
                            foreach (var mod in jsonItem.explicitMods)
                            {
                                if (mod.StartsWith("<"))
                                {
                                    continue;
                                }

                                m++;

                                if (m % 250 == 0)
                                {
                                    Console.Write("{" + itemCount + "}");
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

                                var itemMod = new ItemMod
                                {
                                    Name = mod.GetHashCode().ToString(),
                                    Value = modValue,
                                };

                                itemMods.Add(itemMod);
                            }
                        }

                        if (jsonItem.explicitMods == null && jsonItem.flavourText == null)
                        {
                            item = new Item
                            {
                                Id = jsonItem.id,
                                Ilvl = jsonItem.ilvl,
                                Name = jsonItem.Name,
                                TypeLine = jsonItem.typeLine,
                                Corrupted = jsonItem.corrupted,
                            };
                        }
                        else if (jsonItem.flavourText == null)
                        {
                            item = new MagicalItem
                            {
                                Id = jsonItem.id,
                                Ilvl = jsonItem.ilvl,
                                Name = jsonItem.Name,
                                TypeLine = jsonItem.typeLine,
                                Corrupted = jsonItem.corrupted,
                                ExplicitMods = itemMods,
                            };

                            allItemMods.AddRange(itemMods);
                            //ctx.ItemMods.AddRange(itemMods);
                        }
                        else
                        {
                            item = new UniqueItem
                            {
                                Id = jsonItem.id,
                                Ilvl = jsonItem.ilvl,
                                Name = jsonItem.Name,
                                TypeLine = jsonItem.typeLine,
                                Corrupted = jsonItem.corrupted,
                                ExplicitMods = itemMods,
                            };

                            allItemMods.AddRange(itemMods);
                            //ctx.ItemMods.AddRange(itemMods);
                        }

                        stashTab.Items.Add(item);
                        alltems.Add(item);
                        //ctx.Items.Add(item);
                        addedItemsCount++;
                    }
                }

                Console.WriteLine("Accounts - Exisitng: " + accounts.Count + " Added: " + newAccountsCount
                    + " | Stashes - 'Std': " + standardStashesCount + " HC: " + hardcoreStashesCount + " Empty: " + emptyStashesCount + " Added: " + newStashTabs.Count
                    + " | Items - Added: " + addedItemsCount);
                Console.WriteLine();

                Console.Write("Adding items " + alltems.Count);
                ctx.Items.AddRange(alltems);
                Console.WriteLine("Done");

                Console.Write("Adding mods " + allItemMods.Count);
                ctx.ItemMods.AddRange(allItemMods);
                Console.WriteLine("Done");

                //Console.Write("Adding accounts " + chunkAccounts.Count);
                //ctx.ChunkAccounts.AddRange(chunkAccounts);
                //Console.WriteLine("Done");

                Console.Write("Save changes ");
                ctx.SaveChanges();
                Console.WriteLine("Done");

            }





        }

    }
}
