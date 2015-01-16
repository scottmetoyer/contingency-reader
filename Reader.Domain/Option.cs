using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Domain
{
    public class Option
    {
        [DataMember]
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int OptionID { get; set; }

        [DataMember]
        [Column]
        public string Key { get; set; }

        [DataMember]
        [Column]
        public string Value { get; set; }
    }
}
