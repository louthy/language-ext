using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LanguageExt.Reflect
{
    internal static class Util
    {
        /// <summary>
        /// Emits the IL to instantiate a type of R
        /// </summary>
        public static Func<A, R> CtorOfArity1<A, R>()
        {
            var ctorInfo = typeof(R)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x => x.GetParameters().Length == 1)
                .First();

            var ctorParams = ctorInfo.GetParameters();

            var boundType = typeof(A);
            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, R>)dynamic.CreateDelegate(typeof(Func<A, R>));
        }
    }
}
