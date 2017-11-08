using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using AngleSharp;

namespace MSeBookDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            // this module is copy from following site.

            var url = @"https://blogs.msdn.microsoft.com/mssmallbiz/2017/07/11/largest-free-microsoft-ebook-giveaway-im-giving-away-millions-of-free-microsoft-ebooks-again-including-windows-10-office-365-office-2016-power-bi-azure-windows-8-1-office-2013-sharepo/";
            var output = @"c:\temp\ebook";

            // AngleSharp function
            var config = Configuration.Default.WithDefaultLoader();
            var document = BrowsingContext.New(config).OpenAsync(url).Result;
            var table = document.QuerySelectorAll("table").First();
            var rows = table.Children.First().Children.Skip(1);

            foreach (var row in rows)
            {
                string subDirectory = string.Join("_",
                    row.Children[0].TextContent.Split(Path.GetInvalidFileNameChars()));
                if (!Directory.Exists(Path.Combine(output, subDirectory)))
                {
                    Directory.CreateDirectory(Path.Combine(output, subDirectory));
                }

                foreach (var link in row.QuerySelectorAll("a"))
                {
                    string fileName = $"{row.Children[1].TextContent}.{link.TextContent}";
                    fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
                    using (var httpClient = new HttpClient())
                    {
                        HttpResponseMessage responseMessage = httpClient.GetAsync(link.Attributes["href"].Value).Result;
                        byte[] content = responseMessage.Content.ReadAsByteArrayAsync().Result;
                        // File.WriteAllBytes(Path.Combine(output, subDirectory, fileName), content);
                        Console.WriteLine($"{fileName} saved.");
                    }
                }
            }

            Console.WriteLine("Done!");
            Console.ReadLine(); // pause
        }
    }
}
