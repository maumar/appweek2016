using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ItemIndexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var getStreamTask = httpClient.GetStreamAsync("http://www.pathofexile.com/api/public-stash-tabs");
            getStreamTask.Wait();
            var stream = getStreamTask.Result;

            string value = string.Empty;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                value = reader.ReadToEnd();
            }

            var rootObject = JsonConvert.DeserializeObject<RootObject>(value);

            Console.WriteLine(rootObject.next_change_id);

            foreach (var stash in rootObject.stashes)
            {
                Console.WriteLine("Account Name:" + stash.accountName);
                foreach (var item in stash.items)
                {
                    Console.WriteLine(item.w);
                    Console.WriteLine(item.h);
                    Console.WriteLine(item.ilvl);
                }

            }

            //var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RootObject));
            //var rootObject = (RootObject)serializer.ReadObject(stream);

            //foreach (var stash in rootObject.stashes)
            //{
            //    Console.WriteLine(stash.accountName);
            //}
        }
    }
}
