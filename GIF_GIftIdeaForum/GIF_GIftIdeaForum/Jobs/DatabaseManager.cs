using GIF_GIftIdeaForum.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(-10)]
    public class DatabaseManager : JobBehaviour
    {
        public static class DataDebug
        {
            public static DatabaseManager database;
            public static void GenerateTagsToDb()
            {
                string[] holidays = System.IO.File.ReadAllLines("wwwroot/Debug/Holidays.txt");
                var list = database.giftIdeasDbContext.Tags.ToLookup(a => a.TagName);

                foreach (var item in holidays)
                {
                    if(!list.Contains(item))
                    {
                        TagTable temp = new TagTable() {
                            TagName = item
                        };
                        database.giftIdeasDbContext.Tags.Add(temp);
                    }
                }

                database.giftIdeasDbContext.SaveChanges();
            }
            public static void GenerateItemsToDb()
            {
                string[] items = System.IO.File.ReadAllLines("wwwroot/Debug/RandomItems.txt");
                var tags = database.giftIdeasDbContext.Tags.ToArray();

                int index = 0;
                float da = 0;
                foreach (var item in items)
                {
                    var pos = (tags.Length-1) * 0.5f * Math.Sin(index) + (tags.Length-1) * 0.5f;

                    var randomNum = MathF.Abs((MathF.Sin(da) + 1) * MathF.Log(da));
                    randomNum = MathF.Pow(randomNum, 3);
                    randomNum /= 10;


                    da += 1;
                    database.AddToDatabaseAsync(item, tags[(int)Math.Round(pos)].TagName, null, (int)randomNum);
                    index++;
                }
            }
            public static async Task ClearDebugDatas()
            {
                string[] holidays = System.IO.File.ReadAllLines("wwwroot/Debug/Holidays.txt");
                var debugItems = System.IO.File.ReadAllLines("wwwroot/Debug/RandomItems.txt").ToHashSet();
                var DeleteThese = await database.giftIdeasDbContext.Tags.ToDictionaryAsync(a=>a.TagName, a => a);

                foreach (var holdiday in holidays)
                {
                    var thing = await database.RetrieveDataWithTagAsync(holdiday);
                    if (thing != null)
                    {
                        foreach (var giftIdeas in thing)
                        {
                            if (debugItems.Contains(giftIdeas.GetName()))
                            {
                                await giftIdeas.DeleteFromDb();
                            }
                            else
                            {
                                DeleteThese.Remove(holdiday);
                            }
                        }
                    }

                }

                database.giftIdeasDbContext.Tags.RemoveRange(DeleteThese.Values);
                await database.giftIdeasDbContext.SaveChangesAsync();
            }
            public static void GenerateRandomData()
            {
                GenerateTagsToDb();
                GenerateItemsToDb();
            }
        }
        
        //adding new things to dataset
        //1:33:16 https://www.youtube.com/watch?v=C5cnZ-gZy2I&t=4800s

        private PrimaryDatabase giftIdeasDbContext;
        //Dictionary<string, int> tagsDictionary;

        public override void Run()
        {
            giftIdeasDbContext = PrimaryDatabase;
            DataDebug.database = this;
            //DataDebug.GenerateTagsToDb();
        }
        public override async Task TaskRun()
        {
            var dd = new DirectoryInfo("wwwroot/Images");
            var fileInfo = dd.GetFiles().Where(f => f.Name == "azureBlobTest.jpg").First();
            await AddToDatabaseAsync("Ball", "Valentine's Day", fileInfo, 0); 

            /*
            if (fileInfo == null)
            {
                DebugConsole.Log("No Image Found");
            }
            else
            {
                await UploadImageToBlob(fileInfo);
            }
            var images = await GetImagesFromBlob();
            foreach (var item in images)
            {
                DebugConsole.Log(item.Uri);
            }
            */
        }

        public async Task<List<TagTable>> RetrieveTagsAsync()
        {
            return await giftIdeasDbContext.Tags.ToListAsync<TagTable>();
        }


        public async Task AddToDatabaseAsync(string name, string tag, FileInfo image, int StartingVotes = 0)
        {
            var tagsDictionary = await giftIdeasDbContext.Tags.ToDictionaryAsync(a => a.TagName, o => o.ID);
            var existsTag = tagsDictionary.TryGetValue(tag, out int tagID);
            if (existsTag)
            {
                var items = await RetrieveDataWithTagAsync(tag);
                foreach (var item in items)
                {
                    if (item.GetName() == name)
                    {
                        DebugConsole.Log("Item already exists");
                        return;
                    }
                }

                var uri = await UploadImageToBlob(image);

                GiftIdeasTable giftIdea = new GiftIdeasTable()
                {
                    Name = name,
                    UpVotes = StartingVotes,
                    ImageURI = uri.ToString()
                };

                await giftIdeasDbContext.PresentIdeas.AddAsync(giftIdea);
                await giftIdeasDbContext.SaveChangesAsync();

                var justAddedItem = await giftIdeasDbContext.Entry(giftIdea).GetDatabaseValuesAsync();
                var key = giftIdea.Key;

                TagRelationTable tagRelationTable = new TagRelationTable()
                {
                    GiftKey = key,
                    TagID = tagID
                };
                await giftIdeasDbContext.TagRelations.AddAsync(tagRelationTable);
                await giftIdeasDbContext.SaveChangesAsync();
            }
            else
            {
                DebugConsole.Log("No Tag Found");
            }
        }
        public async Task<List<Gift>> RetrieveDataWithTagAsync(string tag)
        {
            var ob = await giftIdeasDbContext.Tags.ToDictionaryAsync(a => a.TagName, o => o.ID);
            var existsTag = ob.TryGetValue(tag, out int tagID);

            if (existsTag)
            {
                var tagRel = await giftIdeasDbContext.TagRelations.Where(a=>a.TagID == tagID).ToArrayAsync();
                var list = new List<Gift>();

                for (int i = 0; i < tagRel.Length; i++)
                {
                    int key = tagRel[i].GiftKey;
                    var thing = await giftIdeasDbContext.PresentIdeas.FindAsync(key);
                    Gift temp = new Gift(thing.Name, tag, thing.UpVotes, key, thing.ImageURI, giftIdeasDbContext, thing, tagRel[i]);
                    list.Add(temp);
                }
                return list;
            }
            else
            {
                DebugConsole.Log("No Tag Found");
                return null;
            }
        }

        public async Task<Uri> UploadImageToBlob(FileInfo file)
        {
            if (file.Length < 100000)
            {
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());

                string connectionString = CloudData.ConnectionString;
                string container = "images";
                var conClient = new BlobContainerClient(connectionString, container);
                DebugConsole.Log("Uploading Image");

                var blobClient = conClient.GetBlobClient(GuidString + file.Name);
                using (var filestram = File.OpenRead(file.FullName))
                {
                    await blobClient.UploadAsync(filestram);
                    DebugConsole.Log(blobClient.Uri);
                }
                DebugConsole.Log($"{GuidString + file.Name} uploaded");
                return blobClient.Uri;
            }
            else
            {
                DebugConsole.Log("File too large, must be less than 100 KB");
                return null;
            }
        }
        public async Task<List<IListBlobItem>> GetImagesFromBlob()
        {
            BlobContinuationToken blobContinuationToken = null;
            var client = CloudData.account.CreateCloudBlobClient();
            var con = client.GetContainerReference("images");
            BlobResultSegment blobResult = null;
            do
            {
                blobResult = await con.ListBlobsSegmentedAsync(null, blobContinuationToken);
                blobContinuationToken = blobResult.ContinuationToken;

            } while (blobContinuationToken != null);
            return blobResult.Results.ToList();
        }


        public async Task<bool> TagExists(string tag)
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

            //var ob = giftIdeasDbContext.Tags.ToList().TakeWhile(a => a.TagName == tag);
            //var exists = ob.Count() >= 1;

            var ob = await giftIdeasDbContext.Tags.ToDictionaryAsync(a=>a.TagName, a=>a.ID);
            var exists = ob.ContainsKey(tag);
            /*
            var ob = await giftIdeasDbContext.Tags.ToArrayAsync();
            var exists = false;

            Parallel.For(0, ob.Length, (i, loopstate) => { 
                if(ob[i].TagName == tag)
                {
                    exists = true;
                    loopstate.Break();
                }
            });*/
            
            //stopwatch.Stop();
            //DebugConsole.Log(stopwatch.ElapsedMilliseconds);

            return exists;
        }
    }

    public class Gift : IComparable<Gift>
    {
        private string Name;

        public int UpVotes { get; private set; }
        public int ID { get; private set; }
        public string uri { get; private set; }
        private readonly string inTag;
        public TagRelationTable trt { get; private set; }
        private PrimaryDatabase dataBase;
        public GiftIdeasTable dataBaseObject { get; private set; }

        public Gift(string Name, string Tag, int Upvotes, int key, string imageUri, PrimaryDatabase dataBase, GiftIdeasTable dataObject, TagRelationTable tagKey)
        {
            uri = imageUri;
            inTag = Tag;
            this.Name = Name;
            UpVotes = Upvotes;
            ID = key;
            this.dataBase = dataBase;
            dataBaseObject = dataObject;
            trt = tagKey;
        }
        
        public string GetName()
        {
            return Name;
        }
        public async Task IncreaseVotes()
        {
            UpVotes++;
            dataBaseObject.UpVotes = UpVotes;
            await dataBase.SaveChangesAsync();
        }
        public async Task DecreaseVotes()
        {
            UpVotes--;
            dataBaseObject.UpVotes = UpVotes;
            await dataBase.SaveChangesAsync();
        }
        public async Task DeleteFromDb()
        {
            dataBase.TagRelations.Remove(trt);
            dataBase.PresentIdeas.Remove(dataBaseObject);
            await dataBase.SaveChangesAsync();
        }
        public int CompareTo(Gift other)
        {
            if(other == null)
            {
                return 1;
            }
            return this.UpVotes.CompareTo(other.UpVotes);
        }
    }
    public class PrimaryDatabase : DbContext {
        public PrimaryDatabase(DbContextOptions<PrimaryDatabase> options) : base(options)
        {

        }
        public DbSet<GiftIdeasTable> PresentIdeas { get; set; }
        public DbSet<TagTable> Tags { get; set; }
        public DbSet<TagRelationTable> TagRelations { get; set; }
    }
}
