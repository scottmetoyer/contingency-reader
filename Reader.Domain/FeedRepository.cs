using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Reader.Domain.Configuration;
using System.Data.Linq;

namespace Reader.Domain
{
    public class FeedRepository
    {
        private DataContext _context;
        private Table<Feed> _feedTable;
        private Table<Item> _itemTable;

        public IQueryable<Feed> Feeds
        {
            get { return _feedTable; }
        }

        public IQueryable<Item> Items
        {
            get { return _itemTable; }
        }

        public FeedRepository(string connectionString)
        {
            _context = new DataContext(connectionString);
            _feedTable = _context.GetTable<Feed>();
            _itemTable = _context.GetTable<Item>();
        }

        public void SaveFeed(Feed feed)
        {
            if (feed.FeedID == 0)
            {
                _feedTable.InsertOnSubmit(feed);
            }

            _context.SubmitChanges();
        }

        public void SaveItem(Item item)
        {
            if (item.ItemID == 0)
            {
                _itemTable.InsertOnSubmit(item);
            }

            _context.SubmitChanges();
        }

        public void DeleteFeed(Feed feed)
        {
            _feedTable.DeleteOnSubmit(feed);
            _context.SubmitChanges();
        }

        public void DeleteItems(IEnumerable<Item> items)
        {
            _itemTable.DeleteAllOnSubmit(items);
            _context.SubmitChanges();
        }

        public void PurgeItems()
        {
            _context.ExecuteCommand("DELETE FROM Items WHERE IsStarred = 0");
        }

        public void SaveChanges()
        {
            _context.SubmitChanges();
        }
    }
}
