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

    public static class MainC
    {
        private static List<Base> BaseScripts = new List<Base>();

        public static void InitializeMainMethod(Type invokedFrom)
        {
            Type parentType = typeof(Base);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
            System.Diagnostics.Debug.WriteLine("Main Started");

            foreach (Type type in subclasses)
            {
                ConstructorInfo cType = type.GetConstructor(Type.EmptyTypes);
                object _class = cType.Invoke(new object[] { });

                BindToClass MyAttribute =
                        (BindToClass)Attribute.GetCustomAttribute(type, typeof(BindToClass));

                if(MyAttribute != null)
                {
                    if (MyAttribute.parent == invokedFrom)
                    {
                        var method = type.GetMethod("Run");
                        method.Invoke(_class, null);
                    }
                }
                else
                {
                    var method = type.GetMethod("Run");
                    method.Invoke(_class, null);
                }
            }
        }
    }

    public abstract class Base{
        public void DebugLog(String msg)
        {
            System.Diagnostics.Debug.WriteLine("\nConsole Log:\n" + msg + "\n");
        }
        public abstract void Run();
    }

}
