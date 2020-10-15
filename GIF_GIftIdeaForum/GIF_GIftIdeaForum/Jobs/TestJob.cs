using GIF_GIftIdeaForum.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIF_GIftIdeaForum.Jobs
{
    [BindToClass(typeof(IndexModel))] //This ensures that this class is only ran when a specific page is opened
    //without this, the class would run anytime any pages open
    public class TestJob : Base
    {
        public override void Run()
        {
            DebugLog("Invoked"); //this writes to VS console
        }
    }
}
