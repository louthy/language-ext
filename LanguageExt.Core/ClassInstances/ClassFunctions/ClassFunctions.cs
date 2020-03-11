using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace LanguageExt.ClassInstances
{
    internal static class ClassFunctions
    {
        static readonly Dictionary<string, Type> Types; 
        
        static ClassFunctions()
        {
            var types = new Dictionary<string, Type>();
            foreach (var type in ClassInstancesAssembly.AllClassInstances)
            {
                var gens = type.GetGenericArguments();
                var name = type.Name;
                if (!types.ContainsKey(name))
                {
                    types.Add(name, type);
                }
            }

            Types = types;
        }

        public static string RemoveAsync(string name) =>
            name.EndsWith("Async")
                ? name.Substring(0, name.Length - "Async".Length)
                : name;

        public static (string FullName, string Name, Type[] Gens) GetTypeInfo<A>()
        {
            var typeA = typeof(A);
            if (typeA.BaseType?.FullName == "System.Array")
            {
                return ("System.Array", "Array", new[] {typeA.GetElementType()});
            }
            else
            {
                var name = typeA.Name.Split('`')[0];
                var fullName = typeA.FullName.Split('`')[0];
                if (fullName == "System.Collections.Generic.IEnumerable")
                {
                    return ("System.Collections.Generic.Enumerable", "Enumerable", typeA.GetGenericArguments());
                }
                else
                {
                    return (fullName, name, typeA.GetGenericArguments());
                }
            }
        }

        public static Func<Arg, Ret> MakeFunc1<Arg, Ret>(string typeName, string methodName, Type[] gens,
            params (string prefix, string suffix)[] padding)
        {
            var inst = GetInstance(typeName, gens.Length, padding);
            if (inst == null) return null;
            if (gens.Length > 0)
            {
                inst = inst.MakeGenericType(gens);
            }

            var mthd = inst.GetMethod(methodName, new Type[] {typeof(Arg)});
            if (mthd == null) return null;
            var arg = Expression.Parameter(typeof(Arg), "x");
            var lambda = Expression.Lambda<Func<Arg, Ret>>(Expression.Call(Expression.Default(inst), mthd, arg), arg);
            return lambda.Compile();
        }

        public static Func<Arg1, Arg2, Ret> MakeFunc2<Arg1, Arg2, Ret>(string typeName, string methodName, Type[] gens,
            params (string prefix, string suffix)[] padding)
        {
            var inst = GetInstance(typeName, gens.Length, padding);
            if (inst == null) return null;
            if (gens.Length > 0)
            {
                inst = inst.MakeGenericType(gens);
            }

            var mthd = inst.GetMethod(methodName, new Type[] {typeof(Arg1), typeof(Arg2)});
            if (mthd == null) return null;
            var arg1 = Expression.Parameter(typeof(Arg1), "x");
            var arg2 = Expression.Parameter(typeof(Arg2), "y");
            var lambda =
                Expression.Lambda<Func<Arg1, Arg2, Ret>>(Expression.Call(Expression.Default(inst), mthd, arg1, arg2),
                    arg1, arg2);
            return lambda.Compile();
        }

        public static Func<Arg1, Arg2, Arg3, Ret> MakeFunc3<Arg1, Arg2, Arg3,Ret>(string typeName, string methodName, Type[] gens,
            params (string prefix, string suffix)[] padding)
        {
            var inst = GetInstance(typeName, gens.Length, padding);
            if (inst == null) return null;
            if (gens.Length > 0)
            {
                inst = inst.MakeGenericType(gens);
            }

            var mthd = inst.GetMethod(methodName, new Type[] {typeof(Arg1), typeof(Arg2), typeof(Arg3)});
            if (mthd == null) return null;
            var arg1 = Expression.Parameter(typeof(Arg1), "x");
            var arg2 = Expression.Parameter(typeof(Arg2), "y");
            var arg3 = Expression.Parameter(typeof(Arg3), "z");
            var lambda =
                Expression.Lambda<Func<Arg1, Arg2, Arg3, Ret>>(
                    Expression.Call(
                        Expression.Default(inst), 
                        mthd, 
                        arg1, arg2, arg3),
                        arg1, arg2, arg3);
            return lambda.Compile();
        }
        
        
        public static Func<Arg1, Arg2, Arg3, Arg4, Ret> MakeFunc4<Arg1, Arg2, Arg3, Arg4, Ret>(string typeName, string methodName, Type[] gens,
            params (string prefix, string suffix)[] padding)
        {
            var inst = GetInstance(typeName, gens.Length, padding);
            if (inst == null) return null;
            if (gens.Length > 0)
            {
                inst = inst.MakeGenericType(gens);
            }

            var mthd = inst.GetMethod(methodName, new Type[] {typeof(Arg1), typeof(Arg2), typeof(Arg3), typeof(Arg4)});
            if (mthd == null) return null;
            var arg1 = Expression.Parameter(typeof(Arg1), "x");
            var arg2 = Expression.Parameter(typeof(Arg2), "y");
            var arg3 = Expression.Parameter(typeof(Arg3), "z");
            var arg4 = Expression.Parameter(typeof(Arg4), "z");
            var lambda =
                Expression.Lambda<Func<Arg1, Arg2, Arg3, Arg4, Ret>>(
                    Expression.Call(
                        Expression.Default(inst), 
                        mthd, 
                        arg1, arg2, arg3, arg4),
                    arg1, arg2, arg3, arg4);
            return lambda.Compile();
        }

        static Type GetInstance(string name, int arity, params (string prefix, string suffix)[] padding)
        {
            foreach (var pad in padding)
            {
                var pname = arity == 0
                    ? $"{pad.prefix}{name}{pad.suffix}"
                    : $"{pad.prefix}{name}{pad.suffix}`{arity}";

                if(!Types.ContainsKey(pname)) continue;
                return Types[pname];
            }

            return null;
        }
    }
}
