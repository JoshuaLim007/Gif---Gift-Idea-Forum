using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GIF_GIftIdeaForum.Jobs;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GIF_GIftIdeaForum.Pages
{
    public class IndexModel : PageModel
    {
        private static ApplicationDbContext dbContext { get; set; }
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext db)
        {
            dbContext = db;
            _logger = logger;
        }
        public static ApplicationDbContext GetApplicationDbContext()
        {
            return dbContext;
        }

        public void OnGet()
        {
            MainC.InitializeMainMethod(typeof(IndexModel));
        }


    }
}
