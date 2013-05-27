using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using Reader.Domain.Configuration;

namespace Reader.Domain
{
    public class Repository
    {
        private CloudStorageAccount _store = null;
        private CloudTableClient _tableClient = null;
        private CloudTable _feedsTable = null;

        public Repository(string connectionStringName)
        {
             UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");

             if (config == null)
             {
                 throw new Exception("Missing configuration file. Did you rename Default.config to User.config and specify your options?");
             }

            _store = CloudStorageAccount.Parse(config.ConnectionString.Value);
            _tableClient = _store.CreateCloudTableClient();

            // Create tables if they don't exist
            _feedsTable = _tableClient.GetTableReference("feeds");
            _feedsTable.CreateIfNotExists();
        }

        public void SaveFeed(Feed feed)
        {
            TableOperation insert = TableOperation.Insert(feed);
            _feedsTable.Execute(insert);
        }

        public List<Feed> GetFeeds()
        {
            List<Feed> feeds = new List<Feed>();

            TableQuery<Feed> query = new TableQuery<Feed>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "feed"));
            feeds = _feedsTable.ExecuteQuery(query).ToList();

            return feeds;
        }

        public void DropTable()
        {
            _feedsTable.DeleteIfExists();
        }
    }
}
