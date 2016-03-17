using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerModel
{
    public class IndexerStashTab
    {
        public string Id { get; set; }

        public string TabName { get; set; }

        public string League { get; set; }

        // FK -> Account
        public string AccountName { get; set; }
    }
}
