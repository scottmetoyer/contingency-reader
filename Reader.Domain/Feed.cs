using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Reader.Domain
{
    [Table(Name = "Feeds")]
    public class Feed
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int FeedID { get; set; }

        [Column]
        public string DisplayName { get; set; }

        [Column]
        public string URL { get; set; }
    }
}
