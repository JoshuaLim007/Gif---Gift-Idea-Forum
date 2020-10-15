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

/// <summary>
/// DONT TOUCH THIS!!!!!!!
/// </summary>


namespace GIF_GIftIdeaForum
{
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

    public static class MainC
    {
        private class MethodData
        {
            public MethodInfo method;
            public BindToClass attribute;
            public object _class;
        }
        private class MethodDataList
        {
            public List<MethodData> _datas = new List<MethodData>();
        }
        //private static List<Base> BaseScripts = new List<Base>();
        private static SortedList<float, MethodDataList> OrderedExecutionTimes = new SortedList<float, MethodDataList>();
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
                    methodData.method = method;
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
                    methodData.method = method;
                    methodData._class = _class;
                    methodDataList._datas.Add(methodData);

                    OrderedExecutionTimes.Add(orderNum, methodDataList);
                }


            }
        }
        private static void Execute(Type invokedFrom)
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
                            methodData.method.Invoke(methodData._class, null);
                        }
                    }
                    else
                    {
                        methodData.method.Invoke(methodData._class, null);
                    }
                }
            }
        }
        public static void InitializeMainMethod(Type invokedFrom)
        {
            if (NewWeb == false)
            {
                System.Diagnostics.Debug.WriteLine("\nConsole Log:\n" + "Start initialized" + "\n");
                FindBases();
                NewWeb = true;
            }
            Execute(invokedFrom);

        }
    }

    public abstract class JobBehaviour{
        public void DebugLog(String msg)
        {
            System.Diagnostics.Debug.WriteLine("\nConsole Log:\n" + msg + "\n");
        }
        public abstract void Run();
    }

}
