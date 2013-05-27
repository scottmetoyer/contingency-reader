using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Reader.Domain
{
    public class Feed : TableEntity
    {
        public Feed() { }

        public Feed(string url)
        {
            this.PartitionKey = "feed";
            this.RowKey = url;
            this.URL = url;
        }

        public string DisplayName { get; set; }

        public string URL { get; set; }
    }
}
