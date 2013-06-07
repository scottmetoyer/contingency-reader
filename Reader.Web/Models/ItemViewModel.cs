using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reader.Domain;

namespace Reader.Web.Models
{
    public class ItemViewModel
    {
        public Item Item { get; set; }

        public int NextItemID { get; set; }
    }
}