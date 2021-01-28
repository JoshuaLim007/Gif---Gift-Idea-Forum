using com.sun.tools.@internal.ws.processor.model;
using com.sun.tools.javac.jvm;
using GIF_GIftIdeaForum.Pages;
using java.lang;
using javax.jws;
using javax.xml.ws;
using jdk.nashorn.@internal.ir;
using jdk.nashorn.@internal.scripts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(0)]
    public class GiftListManager : JobBehaviour
    {
        public override void Run()
        {
            if(GiftLister.database == null)
                GiftLister.database = FindObjectOfType<DatabaseManager>();
        }
    }

    public static class GiftLister
    {
        public static DatabaseManager database;
        private static string _TagName;
        private static List<Gift> Items;
        public static IndexModel instanceParent = IndexModel.instance;

        public static void SetTag(string TagName)
        {
            _TagName = TagName;
        }
        public static async Task DisplayItems(IJSRuntime jS) {
            if (Items == null)
            {
                Items = await database.RetrieveDataWithTagAsync(_TagName);
                Items = Items.OrderByDescending(o => o.UpVotes).ToList();
            }

            if(Items.Count > 0) {
                await jS.InvokeAsync<List<Items>>("GenerateList", Items);
            }
            else
            {
                DebugConsole.Log("No Items Found");
            }
        }

        public static async Task GoToSubPage(string tag)
        {
            var exists = await database.TagExists(tag);
            if (exists)
            {
                SetTag(tag);
                instanceParent.Response.Redirect("/GiftsPage");
            }
            else
            {
                DebugConsole.Log("Tag does not exist: " + tag);
            }
        }

        [JSInvokableAttribute("CallVotes")]
        public static async Task IncreaseVotes(int id)
        {
            foreach (var item in Items)
            {
                if (item.ID == id)
                {
                    await item.IncreaseVotes();
                }
            }
        }
    }
}

