using GIF_GIftIdeaForum.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(-5)]
    [BindToClass(typeof(IndexModel))]
    public class DatabaseManager : JobBehaviour
    {
        //adding new things to dataset
        //1:33:16 https://www.youtube.com/watch?v=C5cnZ-gZy2I&t=4800s
        private IEnumerable<PresentIdeas> ideas { get; set; }
        private ApplicationDbContext dbContext;
        public override void Run()
        {
            dbContext = IndexModel.GetApplicationDbContext();
            ideas = GetGiftDataFromDataBase();

            ideas = SortDataDescending(ideas);
        }
        public async Task AddToDatabaseAsync(PresentIdeas presentIdeas)
        {
            await dbContext.AddAsync(presentIdeas);
            await dbContext.SaveChangesAsync();
        }
        public void AddToDatabase(PresentIdeas presentIdeas) {
            dbContext.Add(presentIdeas);
            dbContext.SaveChanges();
        }

        public void DeleteDatabase(int key) {
            var idea = dbContext.PresentIdeas.Find(key);
            
            if (idea != null)
            {
                dbContext.PresentIdeas.Remove(idea);
            }
            dbContext.SaveChanges();
        }
        public async Task DeleteDatabaseAsync(int key)
        {
            var idea = await dbContext.PresentIdeas.FindAsync(key);
            if (idea != null) {
                dbContext.PresentIdeas.Remove(idea);
            }

            await dbContext.SaveChangesAsync();
        }

        public void UpdateDatabase(int name, int increaseUpvotesBy) {
            var idea = dbContext.PresentIdeas.Find(name);
            idea.UpVotes += increaseUpvotesBy;
            dbContext.SaveChanges();
        }
        public async Task UpdateDatabaseAsync(int name, int increaseUpvotesBy)
        {
            var idea = await dbContext.PresentIdeas.FindAsync(name);
            idea.UpVotes += increaseUpvotesBy;
            await dbContext.SaveChangesAsync();
        }

        public IEnumerable<PresentIdeas> GetGiftDataFromDataBase()
        {
            var ideas = dbContext.PresentIdeas;
            return ideas;
        }
        public IEnumerable<PresentIdeas> SortDataAscending(IEnumerable<PresentIdeas> ideas)
        {
            return ideas.OrderBy(a => a.UpVotes);
        }
        public IEnumerable<PresentIdeas> SortDataDescending(IEnumerable<PresentIdeas> ideas)
        {
            return ideas.OrderByDescending(a => a.UpVotes);
        }
        public IEnumerable<PresentIdeas> SortDataByNameAscending(IEnumerable<PresentIdeas> ideas)
        {
            return ideas.OrderBy(a => a.Name);
        }
        public IEnumerable<PresentIdeas> SortDataByNameDescending(IEnumerable<PresentIdeas> ideas)
        {
            return ideas.OrderByDescending(a => a.Name);
        }
    }
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<PresentIdeas> PresentIdeas { get; set; }
    }
}
