using GIF_GIftIdeaForum.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Numerics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(-10)]
    public class DatabaseManager : JobBehaviour
    {
        public static class Debug
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
                    database.AddToDatabase(item, tags[(int)Math.Round(pos)].TagName, (int)randomNum);
                    index++;
                }
            }
            public static void ClearDebugDatas()
            {
                string[] holidays = System.IO.File.ReadAllLines("wwwroot/Debug/Holidays.txt");
                var debugItems = System.IO.File.ReadAllLines("wwwroot/Debug/RandomItems.txt").ToHashSet();
                var DeleteThese = database.giftIdeasDbContext.Tags.ToDictionary(a=>a.TagName, a => a);

                foreach (var holdiday in holidays)
                {
                    var thing = database.RetrieveDataWithTag(holdiday);
                    foreach (var giftIdeas in thing)
                    {
                        if (debugItems.Contains(giftIdeas.GetName()))
                        {
                            giftIdeas.DeleteFromDb();
                        }
                        else
                        {
                            DeleteThese.Remove(holdiday);
                        }
                    }
                }

                database.giftIdeasDbContext.Tags.RemoveRange(DeleteThese.Values);
                database.giftIdeasDbContext.SaveChanges();
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
            Debug.database = this;
        }

        public List<TagTable> RetrieveTags()
        {
            return giftIdeasDbContext.Tags.ToList<TagTable>();
        }
        public async Task<List<TagTable>> RetrieveTagsAsync()
        {
            return await giftIdeasDbContext.Tags.ToListAsync<TagTable>();
        }

        public async Task AddToDatabaseAsync(string name, string tag, int StartingVotes = 0)
        {
            var tagsDictionary = await giftIdeasDbContext.Tags.ToDictionaryAsync(a => a.TagName, o => o.ID);

            var existsTag = tagsDictionary.TryGetValue(tag, out int tagID);
            if (existsTag)
            {

                var items = await RetrieveDataWithTagAsync(tag);

                foreach (var item in items)
                {
                    if(item.GetName() == name)
                    {
                        DebugLog("Item already exists");
                        return;
                    }
                }


                GiftIdeasTable giftIdea = new GiftIdeasTable()
                {
                    Name = name,
                    UpVotes = StartingVotes,
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
                DebugLog("No Tag Found");
            }
        }
        public void AddToDatabase(string name, string tag, int StartingVotes = 0) {
            var tagsDictionary = giftIdeasDbContext.Tags.ToDictionary(a => a.TagName, o => o.ID);

            var existsTag = tagsDictionary.TryGetValue(tag, out int tagID);
            if (existsTag)
            {
                var items = RetrieveDataWithTag(tag);
                foreach (var item in items)
                {
                    if(item.GetName() == name)
                    {
                        DebugLog("Item already exists");
                        return;
                    }
                }


                GiftIdeasTable giftIdea = new GiftIdeasTable()
                {
                    Name = name,
                    UpVotes = StartingVotes,
                };
                giftIdeasDbContext.PresentIdeas.Add(giftIdea);
                giftIdeasDbContext.SaveChanges();

                var justAddedItem = giftIdeasDbContext.Entry(giftIdea).GetDatabaseValues();

                var key = giftIdea.Key;
                TagRelationTable tagRelationTable = new TagRelationTable()
                {
                    GiftKey = key,
                    TagID = tagID
                };
                giftIdeasDbContext.TagRelations.Add(tagRelationTable);
                giftIdeasDbContext.SaveChanges();
            }
            else
            {
                DebugLog("No Tag Found");
            }
        }

        public List<Gift> RetrieveDataWithTag(string tag)
        {
            var ob = giftIdeasDbContext.Tags.ToDictionary(a => a.TagName, o => o.ID);
            var existsTag = ob.TryGetValue(tag, out int tagID);
            if (existsTag)
            {
                var tagRel = giftIdeasDbContext.TagRelations.Where(a=>a.TagID == tagID).ToArray();

                var list = new List<Gift>();

                for (int i = 0; i < tagRel.Length; i++)
                {
                    int key = tagRel[i].GiftKey;
                    var thing = giftIdeasDbContext.PresentIdeas.Find(key);

                    Gift temp = new Gift(thing.Name, tag, thing.UpVotes, key, giftIdeasDbContext, thing, tagRel[i]);
                    list.Add(temp);
                }
                return list;
            }
            else
            {
                DebugLog("No Tag Found");
                return null;
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
                    Gift temp = new Gift(thing.Name, tag, thing.UpVotes, key, giftIdeasDbContext, thing, tagRel[i]);
                    list.Add(temp);
                }
                return list;
            }
            else
            {
                DebugLog("No Tag Found");
                return null;
            }
        }

        public IEnumerable<T> GetPureDataFromDb<T>()
        {
            var d = typeof(T);
            if(d == typeof(GiftIdeasTable))
            {
                var list = giftIdeasDbContext.PresentIdeas;
                return (IEnumerable<T>)list;
            }
            else if (d == typeof(TagTable))
            {
                var list = giftIdeasDbContext.Tags;
                return (IEnumerable<T>)list;
            }
            else
            {
                var list = giftIdeasDbContext.TagRelations;
                return (IEnumerable<T>)list;
            }
        }
    }
    public class Gift : IComparable<Gift>
    {
        private string Name;

        public int UpVotes { get; private set; }


        private readonly string inTag;
        private readonly int Index;
        public TagRelationTable trt { get; private set; }
        private PrimaryDatabase dataBase;
        public GiftIdeasTable dataBaseObject { get; private set; }

        public Gift(string Name, string Tag, int Upvotes, int key, PrimaryDatabase dataBase, GiftIdeasTable dataObject, TagRelationTable tagKey)
        {
            inTag = Tag;
            this.Name = Name;
            UpVotes = Upvotes;
            Index = key;
            this.dataBase = dataBase;
            dataBaseObject = dataObject;
            trt = tagKey;
        }
        
        public string GetName()
        {
            return Name;
        }
        public void IncreaseVotes()
        {
            UpVotes++;
            dataBaseObject.UpVotes = UpVotes;
            dataBase.SaveChanges();
        }
        public void DecreaseVotes()
        {
            UpVotes--;
            dataBaseObject.UpVotes = UpVotes;
            dataBase.SaveChanges();
        }
        public void DeleteFromDb()
        {
            dataBase.TagRelations.Remove(trt);
            dataBase.PresentIdeas.Remove(dataBaseObject);
            dataBase.SaveChanges();
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
