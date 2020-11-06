using GIF_GIftIdeaForum.Pages;
using javax.jws;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
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

        public static void SetTag(string TagName)
        {
            _TagName = TagName;
        }
        public static async Task DisplayItems(IJSRuntime jS) {
            var items = database.RetrieveDataWithTag(_TagName);
            foreach (var item in items)
            {
                database.DebugLog(item.GetName());
            }
            await jS.InvokeVoidAsync("GenerateList");
        }
    }
}
