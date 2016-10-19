using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LanguageExt.Reflect
{
    internal static class Util
    {
        /// <summary>
        /// Emits the IL to instantiate a NewType
        /// </summary>
        /// <returns></returns>
        public static Func<A, NEWTYPE> CreateDynamicConstructor<A, NEWTYPE>()
        {
            var ctorInfo = typeof(NEWTYPE)
                .GetTypeInfo()
                .GetConstructors()
                .Where(x => x.GetParameters().Length == 1)
                .First();
#if COREFX
            return fun((T x) => (NEWTYPE)Activator.CreateInstance(typeof(NEWTYPE), new object[1] { x }));
#else
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

            return (Func<A, NEWTYPE>)dynamic.CreateDelegate(typeof(Func<A, NEWTYPE>));
#endif
        }
    }
}
