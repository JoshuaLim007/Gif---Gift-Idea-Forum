using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GIF_GIftIdeaForum.Pages;

/*shit to do
 * method so that josh can pull back the sorted data
 */

namespace GIF_GIftIdeaForum.Jobs
{
    //[ExecutionOrder(0)]
    //[BindToClass(typeof(IndexModel))]
    /*
     public class SortDataSet : JobBehaviour
    {
        public override void Run() //pulls the structs from database
        {
            var database = FindObjectOfType<DatabaseManager>();
            var dataSet = database.GetGiftDataFromDataBase().ToList();
            SortData_Main(ref dataSet);
            for (int i = 0; i < dataSet.Count; i++)
            {
                DebugLog("Present: " + dataSet[i].Name + " Votes: " + dataSet[i].UpVotes);
            }
        }

        private static int QuickPartition(ref List<PresentIdeas>DataVotes, int low, int high)
        {
            //DataVotes[i].UpVotes
            
            int i = low - 1;
            int pivot = DataVotes[high].UpVotes;

            for (int j = low; j < high; j++)
            {
                if (DataVotes[j].UpVotes < pivot)
                {
                    i = i + 1;
                    int temp = DataVotes[i].UpVotes;
                    DataVotes[i].UpVotes = DataVotes[j].UpVotes;
                    DataVotes[j].UpVotes = temp;
                }
            }
            int temp2 = DataVotes[i + 1].UpVotes;
            DataVotes[i + 1].UpVotes = DataVotes[high].UpVotes;
            DataVotes[high].UpVotes = temp2;

            return (i + 1);
        }
        private static void Quicksort(ref List<PresentIdeas>DataVotes, int low, int high)
        {
            if (low < high)
            {
                int pi = QuickPartition(ref DataVotes, low, high);

                Quicksort(ref DataVotes, low, pi - 1);
                Quicksort(ref DataVotes, pi + 1, high);
            }
        }

        private static void print(ref List<PresentIdeas> DataVotes, int element)
        {
            for (int i = 0; i < element; i++)
                Console.Write(DataVotes[i].UpVotes + " ");
        }

        private static void SortData_Main(ref List<PresentIdeas> DataVotes)
        {
            int element = DataVotes.Count;

            Quicksort(ref DataVotes, 0, element - 1);
            print(ref DataVotes, element);
        }
    }
     */
}
