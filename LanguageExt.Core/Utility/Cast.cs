using System;
using System.Linq;
using System.Reflection;

namespace LanguageExt
{
    internal static class Cast
    {
        static Set<int> cache = new Set<int>();

        public static bool IsCastableTo(this Type from, Type to)
        {
            int key = 0;
            unchecked
            {
                key = from.GetHashCode() * to.FullName.GetHashCode() * 13;
            }

            if (cache.Contains(key)) return true;

            if (to.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo()))
            {
                cache = cache.Add(key);
                return true;
            }
            if (to.GetTypeInfo()
                  .DeclaredMethods
                  .Any(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.GetParameters()[0].ParameterType == from))
            {
                cache = cache.Add(key);
                return true;
            }
            if (from.GetTypeInfo()
                    .DeclaredMethods
                    .Any(m => m.ReturnType == to && (m.Name == "op_Implicit" || m.Name == "op_Explicit")))
            {
                cache = cache.Add(key);
                return true;
            }
            return false;
        }
    }
}
