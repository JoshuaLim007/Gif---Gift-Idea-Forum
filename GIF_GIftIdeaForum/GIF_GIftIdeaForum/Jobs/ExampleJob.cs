using GIF_GIftIdeaForum.Pages;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GIF_GIftIdeaForum.Jobs
{
    ///////////////////////
    //Example Code//
    ///////////////////////

    /*
    [ExecutionOrder(0.25f)] //Requires Execution Order Number, this tells the JobBehavaiour when to run it. Lower numbers go first.
    [BindToClass(typeof(IndexModel))] //This ensures that this class is only ran when a specific page is opened, in this case when Index page is opened
    public class ExampleJob : JobBehaviour
    {
        //DatabaseManager dm;
        public override void Run() //This is called once
        {
            dm = FindObjectOfType<DatabaseManager>(); //this will get the class instance of classes we will make in the future
                                                      //with this you can get all methods and variabls
                                                      
            var list = dm.GetGiftDataFromDataBase(); //I created a get gift from database so you can get all the items from the database
            DebugLog("Sync: " + list[0].Name);
        }
        public override async Task TaskRun() //this is new Run method but it is called async from the website. This makes it so that the seperate code could be ran at the same time
        {
            //Async methods must be called in the TaskRun() method

            var list = await dm.GetGiftDataFromDataBaseAsync(); //This is basically same thing as the other one, but this is async. So when we have a large databse, it won't load forever but rather in the background
            DebugLog("Async: " + list[0].Name); //This will write to the compilers command output
        }
    }*/

    ///////////////////////
    ///////////////////////
    ///////////////////////
}
