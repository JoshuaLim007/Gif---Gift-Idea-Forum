using System;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using GIF_GIftIdeaForum.Jobs;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

/// <summary>
/// DONT TOUCH THIS!!!!!!!
/// </summary>


namespace GIF_GIftIdeaForum
{
    public class TagRelationTable {
        [Key]
        public int RelationID { get; set; }
        public int TagID { get;set;}
        public int GiftKey { get; set; }
    }
    public class TagTable { 
        [Key]
        public int ID { get; set; }
        public string TagName { get; set; }
    
    }
    public class GiftIdeasTable
    {
        [Key]
        public int Key { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int UpVotes { get; set; }
    }

    public static class ExtensionMethods
    {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);
            if (task != null)
            {
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty.GetValue(task);
            }

            return null;
        }
    }

    public static class MainC
    {
        private class MethodData
        {
            public List<MethodInfo> method = new List<MethodInfo>();
            public BindToClass attribute;
            public object _class;
        }
        private class MethodDataList
        {
            public List<MethodData> _datas = new List<MethodData>();
        }
        //private static List<Base> BaseScripts = new List<Base>();
        private static SortedList<float, MethodDataList> OrderedExecutionTimes = new SortedList<float, MethodDataList>();
        public static Dictionary<Type, object> AllInstanced = new Dictionary<Type, object>();

        private static bool NewWeb { get; set; }
        private static void FindBases()
        {
            Type parentType = typeof(JobBehaviour);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

            float orderNum = 0;
            foreach (Type type in subclasses)
            {
                ConstructorInfo cType = type.GetConstructor(Type.EmptyTypes);
                object _class = cType.Invoke(new object[] { });

                AllInstanced.Add(type, _class);

                BindToClass BindAttribute =
                        (BindToClass)Attribute.GetCustomAttribute(type, typeof(BindToClass));
                ExecutionOrder ExecutionAttribute =
                    (ExecutionOrder)Attribute.GetCustomAttribute(type, typeof(ExecutionOrder));

                if (ExecutionAttribute != null)
                {
                    orderNum = ExecutionAttribute.orderTime;
                }
                else
                {
                    throw new Exception("No execution order found at: " + _class.ToString());
                }


                if (OrderedExecutionTimes.ContainsKey(orderNum))
                {
                    MethodData methodData = new MethodData();
                    methodData.attribute = BindAttribute;
                    var method = type.GetMethod("Run");
                    methodData.method.Add(method);
                    method = type.GetMethod("TaskRun");
                    if (method != null)
                    {
                        methodData.method.Add(method);
                    }
                    methodData._class = _class;

                    var _list = OrderedExecutionTimes[orderNum];
                    _list._datas.Add(methodData);
                }
                else
                {
                    MethodDataList methodDataList = new MethodDataList();

                    MethodData methodData = new MethodData();
                    methodData.attribute = BindAttribute;
                    var method = type.GetMethod("Run");
                    methodData.method.Add(method);
                    method = type.GetMethod("TaskRun");

                    if (method != null)
                    {
                        methodData.method.Add(method);
                    }

                    methodData._class = _class;
                    methodDataList._datas.Add(methodData);

                    OrderedExecutionTimes.Add(orderNum, methodDataList);
                }


            }
        }
        private static async Task Execute(Type invokedFrom)
        {
            foreach (var scripts in OrderedExecutionTimes)
            {
                var methodDataList = scripts.Value;

                for (int i = 0; i < methodDataList._datas.Count; i++)
                {
                    var methodData = methodDataList._datas[i];
                    if (methodData.attribute != null)
                    {
                        if (methodData.attribute.parent == invokedFrom)
                        {
                            for (int d = 0; d < methodData.method.Count; d++)
                            {
                                var name = methodData.method[d].Name;
                                switch (name)
                                {
                                    case ("TaskRun"):

                                        await methodData.method[d].InvokeAsync(methodData._class, null);

                                        break;
                                    case ("Run"):
                                        methodData.method[d].Invoke(methodData._class, null);
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int d = 0; d < methodData.method.Count; d++)
                        {
                            var name = methodData.method[d].Name;
                            switch (name)
                            {
                                case ("TaskRun"):
                                    await methodData.method[d].InvokeAsync(methodData._class, null);

                                    break;
                                case ("Run"):
                                    methodData.method[d].Invoke(methodData._class, null);
                                    break;
                            }
                        }
                    }
                }
            }
        }
        public static async Task InitializeMainMethod(Type invokedFrom)
        {
            if (NewWeb == false)
            {
                System.Diagnostics.Debug.WriteLine("\nConsole Log:\n" + "Start initialized" + "\n");
                FindBases();
                NewWeb = true;
            }
            await Execute(invokedFrom);

        }
        public static T FindObjectOfType<T>()
        {
            object instance;
            MainC.AllInstanced.TryGetValue(typeof(T), out instance);
            return (T)Convert.ChangeType(instance, typeof(T));
        }
    }

    public class BindToClass : System.Attribute
    {
        public Type parent;

        public BindToClass(Type parent)
        {
            this.parent = parent;
        }
    }
    public class ExecutionOrder : System.Attribute
    {
        public float orderTime;

        public ExecutionOrder(float orderTime)
        {
            this.orderTime = orderTime;
        }
    }
    public abstract class JobBehaviour{
        public void DebugLog(object msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
        public abstract void Run();


        public virtual Task TaskRun()
        {
            return null;
        }
        public T FindObjectOfType<T>()
        {
            return MainC.FindObjectOfType<T>();
        }
    }

}
