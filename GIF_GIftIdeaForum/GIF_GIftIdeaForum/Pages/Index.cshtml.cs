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
        public static IndexModel instance{ get; private set; }
        //private static PrimaryDatabase _GiftIdeaDataContext { get; set; }

        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, PrimaryDatabase db)
        {
            Behaviour.PrimaryDatabase = db;
            _logger = logger;
            instance = this;
        }

        public async Task OnGet()
        {
            await MainC.Start(typeof(IndexModel), this);
            //await GiftLister.GoToSubPage("Christmas Day");
        }

        public async Task OnPostGoToSubPage(string SubPageTag)
        {
            //the button calls this function redirect to CHristmas
            GiftLister.GoToSubPage(SubPageTag);
        }
    }
}
