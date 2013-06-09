using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Reader.Domain
{
    [DataContract]
    [Table(Name = "Feeds")]
    public class Feed
    {
        [DataMember]
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int FeedID { get; set; }

        [DataMember]
        [Column]
        public string DisplayName { get; set; }

        [DataMember]
        [Column]
        public string URL { get; set; }

        [Column(UpdateCheck = UpdateCheck.Never)]
        public Binary Favicon { get; set; }

        [Column]
        public string BlogURL { get; set; }

        [Column]
        public DateTime? LastRefresh { get; set; }

        public Feed()
        {
            this.Favicon = new byte[0];
            this.URL = string.Empty;
            this.BlogURL = string.Empty;
            this.DisplayName = string.Empty;
        }
    }
}
