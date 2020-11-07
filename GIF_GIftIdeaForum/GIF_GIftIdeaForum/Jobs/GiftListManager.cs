using com.sun.tools.@internal.ws.processor.model;
using com.sun.tools.javac.jvm;
using GIF_GIftIdeaForum.Pages;
using javax.jws;
using javax.xml.ws;
using jdk.nashorn.@internal.scripts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIF_GIftIdeaForum.Jobs
{
    [ExecutionOrder(0)]
    [BindToClass(typeof(GiftsPageModel))]
    public class GiftListManager : JobBehaviour
    {
        public override void Run()
        {
            var data = GiftLister.database = FindObjectOfType<DatabaseManager>();
        }
    }

    public static class GiftLister
    {
        public static DatabaseManager database;
        private static string _TagName;
        private static List<Gift> Items;
        public static IndexModel instanceParent;

        public static void SetTag(string TagName)
        {
            _TagName = TagName;
        }
        public static async Task DisplayItems(IJSRuntime jS) {
            if (Items == null)
            {
                Items = database.RetrieveDataWithTag(_TagName);
                Items = Items.OrderBy(o => o.UpVotes).ToList();
            }

            await jS.InvokeAsync<List<Items>>("GenerateList", Items);
        }

        public static void GoToSubPage(string tag)
        {
            GiftLister.SetTag(tag);
            instanceParent.Response.Redirect("/GiftsPage");
        }
    }
}
