using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Domain
{
    [Table(Name = "Items")]
    public class Item
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ItemID { get; set; }

        [Column]
        public int FeedID { get; set; }

        [Column]
        public string Title { get; set; }

        [Column]
        public DateTime? PublishDate { get; set; }

        [Column]
        public string Content { get; set; }

        [Column]
        public string URL { get; set; }

        [Column]
        public bool IsRead { get; set; }

        [Column]
        public bool IsStarred { get; set; }

        [Column]
        public DateTime FetchDate { get; set; }
    }
}
