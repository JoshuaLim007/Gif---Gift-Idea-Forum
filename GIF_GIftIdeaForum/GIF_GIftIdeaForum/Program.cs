using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GIF_GIftIdeaForum.Pages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace GIF_GIftIdeaForum
{
    class CloudData
    {
        public static CloudStorageAccount account;
        public readonly static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=giftideaforumimages;AccountKey=Iuksee4yUAZy0wQqicVDXYGQ8J7I9vyUNbnrM5QqRoQUSnMHiRxi2iNltj5k00WzV858aq8l4NpeZDA6nAvK/Q==;EndpointSuffix=core.windows.net";
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            SetUpBLobAccount();
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        private static void SetUpBLobAccount()
        {
            string connectionString = CloudData.ConnectionString;
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                //System.Diagnostics.Debug.WriteLine("success");
                CloudData.account = storageAccount;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
