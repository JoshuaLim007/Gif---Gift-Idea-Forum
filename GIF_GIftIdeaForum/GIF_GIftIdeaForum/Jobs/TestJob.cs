using GIF_GIftIdeaForum.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(0)] //Requires Execution Order Number
    [BindToClass(typeof(IndexModel))] //This ensures that this class is only ran when a specific page is opened
    //without this, the class would run anytime any pages open
    public class TestJob : Base
    {
        public override void Run()
        {
            DebugLog("Invoked 0"); //this writes to VS console
        }
    }

    [ExecutionOrder(1)]
    [BindToClass(typeof(IndexModel))]
    public class TestJobSecond : Base
    {
        public override void Run()
        {
            DebugLog("Invoked 1"); 
        }
    }

    [ExecutionOrder(2)]
    [BindToClass(typeof(IndexModel))]
    public class TestJobThird : Base
    {
        public override void Run()
        {
            DebugLog("Invoked 2");
        }
    }

}
