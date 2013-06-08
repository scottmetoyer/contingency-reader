using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Reader.Domain;
using Reader.Domain.Configuration;

namespace Reader.Fetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fetching list of feeds to update...");
            var client = new WebClient();

            try
            {
                UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
                client.Headers.Add(HttpRequestHeader.Authorization, config.AuthToken.Value);

                string baseUrl = config.ServiceUrl.Value;
                var json = client.DownloadData(baseUrl + "feeds");

                // Deserialize to a list of feeds
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Feed>));
                List<Feed> feeds = (List<Feed>)serializer.ReadObject(new MemoryStream(json));

                foreach (var feed in feeds)
                {
                    Console.WriteLine("Fetching new articles in feed: " + feed.DisplayName);
                    string parameters = "=" + feed.FeedID.ToString();

                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    try
                    {
                        client.UploadString(baseUrl + "feeds/refresh", "POST", parameters);
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }

                Console.WriteLine("Done fetching new articles. Happy reading!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
