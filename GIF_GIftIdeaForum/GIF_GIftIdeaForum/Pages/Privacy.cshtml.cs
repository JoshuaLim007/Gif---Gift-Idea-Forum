using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GIF_GIftIdeaForum.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GIF_GIftIdeaForum.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            await MainC.Start(typeof(PrivacyModel));
        }
    }
}
