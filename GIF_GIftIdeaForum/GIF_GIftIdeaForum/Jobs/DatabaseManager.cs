using GIF_GIftIdeaForum.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(-5)]
    [BindToClass(typeof(IndexModel))]
    public class DatabaseManager : JobBehaviour
    {
        private IEnumerable<PresentIdeas> ideas { get; set; }
        private ApplicationDbContext dbContext;
        public override void Run()
        {
            dbContext = IndexModel.GetApplicationDbContext();
        }

        public async Task<List<PresentIdeas>> GetGiftDataFromDataBaseAsync()
        {
            ideas = await dbContext.PresentIdeas.ToListAsync();
            return ideas.ToList();
        }
        public List<PresentIdeas> GetGiftDataFromDataBase()
        {
            ideas = dbContext.PresentIdeas.ToList();
            return ideas.ToList();
        }

        public static void AddGiftIdeaToDataBase(string Name)
        {

        }
    }
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<PresentIdeas> PresentIdeas { get; set; }
    }
}
