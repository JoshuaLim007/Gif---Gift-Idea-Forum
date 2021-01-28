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


/// <summary>
/// DONT TOUCH THIS!!!!!!!
/// </summary>


namespace GIF_GIftIdeaForum
{
    #region Tables
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
        [Required]
        public string ImageURI { get; set; }
    }
    #endregion

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
        [Obsolete]private class MethodData
        {
            public List<MethodInfo> method = new List<MethodInfo>();
            public BindToModel attribute;
            public object _class;
        }
        [Obsolete]private class MethodDataList
        {
            public List<MethodData> _datas = new List<MethodData>();
        }

        [Obsolete]private static SortedList<float, MethodDataList> OrderedExecutionTimes = new SortedList<float, MethodDataList>();
        
        public static Dictionary<Type, JobBehaviour> Instances = new Dictionary<Type, JobBehaviour>();
        private static Dictionary<Type, SortedList<float, JobBehaviour>> InstanceBindedOrdered = new Dictionary<Type, SortedList<float, JobBehaviour>>();
        private static SortedList<float, JobBehaviour> InstanceNonBindedOrdered = new SortedList<float, JobBehaviour>();

        private static bool NewWeb { get; set; }


        private static void FindBases()
        {
            Type parentType = typeof(JobBehaviour);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

            DebugConsole.Log("\n" + subclasses);

            //float orderNum = 0;
            foreach (var type in subclasses)
            {

                ConstructorInfo cType = type.GetConstructor(Type.EmptyTypes);
                object _class = cType.Invoke(new object[] { });

                var typeChanged = Cast(_class, type);
                Instances.Add(type, typeChanged);

                BindToModel BindAttribute =
                    (BindToModel)Attribute.GetCustomAttribute(type, typeof(BindToModel));
                ExecutionOrder ExecutionAttribute =
                    (ExecutionOrder)Attribute.GetCustomAttribute(type, typeof(ExecutionOrder));

                float order = 0;
                if (ExecutionAttribute != null)
                {
                    order = ExecutionAttribute.orderTime;
                }
                else
                {
                    order = 0;
                }

                if (BindAttribute != null)
                {
                    if (!InstanceBindedOrdered.ContainsKey(BindAttribute.parent))
                    {
                        InstanceBindedOrdered.Add(BindAttribute.parent, new SortedList<float, JobBehaviour>());
                        InstanceBindedOrdered[BindAttribute.parent].Add(order, typeChanged);
                    }
                    else
                    {
                        InstanceBindedOrdered[BindAttribute.parent].Add(order, typeChanged);
                    }
                }
                else
                {
                    InstanceNonBindedOrdered.Add(order, typeChanged);
                }

                //The below code is absolute trash.. way too slow.. reflections not good!
                //////////////////////////////////////////////////////////////////////
                #region
                //Instances.Add(type, _class);

                /*

                if (ExecutionAttribute != null)
                {
                    orderNum = ExecutionAttribute.orderTime;
                }
                else
                {
                    throw new Exception("No execution order found at: " + _class.ToString());
                }

                //performance not good
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

                */

                #endregion
                //////////////////////////////////////////////////////////////////////
            }

        }

        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        private static async Task ExecuteInstances(Type invokedFrom)
        {

            //var temp = FindObjectOfType<GiftListManager>();
            //temp.Run();

            var available = InstanceBindedOrdered.TryGetValue(invokedFrom, out SortedList<float, JobBehaviour> list);

            if (list != null)
            {
                foreach (var item in list)
                {
                    var instance = item.Value;
                    instance.Run();
                    await instance.TaskRun();
                }
            }

            if (InstanceNonBindedOrdered != null)
            {
                foreach (var item in InstanceNonBindedOrdered)
                {
                    var instance = item.Value;
                    instance.Run();
                    await instance.TaskRun();
                }
            }
        }


        [Obsolete("Use ExecuteInstances")]
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


        public static async Task Start(Type invokedFrom)
        {
            if (Behaviour.previousPage != invokedFrom)
            {
                Behaviour.previousPage = invokedFrom;
                if (NewWeb == false)
                {
                    DebugConsole.Log("\nConsole Log:\n" + "Start initialized" + "\n");

                    FindBases();

                    NewWeb = true;
                }
                await ExecuteInstances(invokedFrom);
            }
        }
        public static T FindObjectOfType<T>()
        {
            JobBehaviour instance;
            Instances.TryGetValue(typeof(T), out instance);
            return (T)Convert.ChangeType(instance, typeof(T));
        }
    }

    #region Framework
    public static class DebugConsole
    {
        public static void Log(object msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }

    public class BindToModel : System.Attribute
    {
        public Type parent;

        public BindToModel(Type parent)
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

    public class Behaviour
    {
        public static Type previousPage;
        public static PrimaryDatabase PrimaryDatabase;
    }
    public abstract class JobBehaviour : Behaviour{

        public abstract void Run();

        public virtual Task TaskRun()
        {
            return Task.FromResult(0);
        }
        public T FindObjectOfType<T>()
        {
            return MainC.FindObjectOfType<T>();
        }

    }
    #endregion
}
