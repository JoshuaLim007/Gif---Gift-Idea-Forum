using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GIF_GIftIdeaForum.Jobs;
using java.lang;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace GIF_GIftIdeaForum.Pages
{
    public class GiftsPageModel : PageModel
    {
        private readonly ILogger<GiftsPageModel> _logger;

        public GiftsPageModel(ILogger<GiftsPageModel> logger, PrimaryDatabase db)
        {
            _logger = logger;
            Behaviour.PrimaryDatabase = db;
        }
        public async Task OnGet()
        {
            await MainC.Start(typeof(GiftsPageModel), this);
        }
        public async Task OnPostListitems(IJSRuntime jS)
        {
            await GiftLister.DisplayItems(jS);
        }
    }
}
