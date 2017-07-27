using System;
using System.Linq;
using System.Reflection;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class Cast
    {
        static Set<int> castCache = new Set<int>();
        static Set<int> implicitCache = new Set<int>();
        static Set<int> exlicitCache = new Set<int>();

        public static bool IsCastableTo(this Type from, Type to)
        {
            int key = 0;
            unchecked
            {
                key = from.GetHashCode() * to.GetHashCode() * 13;
            }

            if (castCache.Contains(key)) return true;

            if (to.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo()))
            {
                castCache = castCache.Add(key);
                return true;
            }
            if (to.GetTypeInfo()
                  .DeclaredMethods
                  .Any(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.GetParameters()[0].ParameterType == from))
            {
                castCache = castCache.Add(key);
                return true;
            }
            if (from.GetTypeInfo()
                    .DeclaredMethods
                    .Any(m => m.ReturnType == to && (m.Name == "op_Implicit" || m.Name == "op_Explicit")))
            {
                castCache = castCache.Add(key);
                return true;
            }
            return false;
        }

        public static Option<A> CastTo<A>(this object from)
        {
            if (from == null) return None;
            if (from is A) return (A)from;

            return IL.Func1<A, A>(from.GetType(), m => m.Name == "op_Implicit")
                     .Map(f => f(from));

            //if (typeof(A).GetTypeInfo()
            //             .DeclaredMethods
            //             .Any(m => m.Name == "op_Implicit") && m.GetParameters()[0].ParameterType == from))
            //{
            //    castCache = castCache.Add(key);
            //    return true;
            //}

                //var fromType = from.GetType();


                //int key = 0;
                //var fromType = from.GetType();

                //unchecked
                //{
                //    key = from.GetHashCode() * typeof(A).GetHashCode() * 13;
                //}

                //if (castCache.Contains(key))
                //{
                //    return (A)
                //}

                //if (to.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo()))
                //{
                //    castCache = castCache.Add(key);
                //    return true;
                //}
                //if (to.GetTypeInfo()
                //      .DeclaredMethods
                //      .Any(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.GetParameters()[0].ParameterType == from))
                //{
                //    castCache = castCache.Add(key);
                //    return true;
                //}
                //return false;
        }

    }
}
