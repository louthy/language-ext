using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class IL
    {
        /// <summary>
        /// Emits the IL to instantiate a type of R with a single argument to 
        /// the constructor
        /// </summary>
        public static Func<A, R> Ctor<A, R>(Func<ConstructorInfo, bool> ctorPred = null)
        {
            ctorPred = ctorPred ?? (_ => true);

            var ctorInfo = typeof(R)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    return ctorPred(x);
                })
                .FirstOrDefault();

            if (ctorInfo == null) throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}");

            var ctorParams = ctorInfo.GetParameters();

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

        /// <summary>
        /// Emits the IL to instantiate a type of R with two arguments to 
        /// the constructor
        /// </summary>
        public static Func<A, B, R> Ctor<A, B, R>(Func<ConstructorInfo, bool> ctorPred = null)
        {
            ctorPred = ctorPred ?? (_ => true);

            var ctorInfo = typeof(R)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    var ps = x.GetParameters();
                    if (ps.Length != 2) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    return ctorPred(x);
                })
                .FirstOrDefault();

            if (ctorInfo == null) throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}");

            var ctorParams = ctorInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, R>)dynamic.CreateDelegate(typeof(Func<A, B, R>));
        }

        /// <summary>
        /// Emits the IL to instantiate a type of R with three arguments to 
        /// the constructor
        /// </summary>
        public static Func<A, B, C, R> Ctor<A, B, C, R>(Func<ConstructorInfo, bool> ctorPred = null)
        {
            ctorPred = ctorPred ?? (_ => true);

            var ctorInfo = typeof(R)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    var ps = x.GetParameters();
                    if (ps.Length != 3) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    if (ps[2].ParameterType != typeof(C)) return false;
                    return ctorPred(x);
                })
                .FirstOrDefault();

            if (ctorInfo == null) throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}");

            var ctorParams = ctorInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, C, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, R>));
        }

        /// <summary>
        /// Emits the IL to instantiate a type of R with four arguments to 
        /// the constructor
        /// </summary>
        public static Func<A, B, C, D, R> Ctor<A, B, C, D, R>(Func<ConstructorInfo, bool> ctorPred = null)
        {
            ctorPred = ctorPred ?? (_ => true);

            var ctorInfo = typeof(R)
                .GetTypeInfo()
                .DeclaredConstructors
                .Where(x =>
                {
                    var ps = x.GetParameters();
                    if (ps.Length != 4) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    if (ps[1].ParameterType != typeof(B)) return false;
                    if (ps[2].ParameterType != typeof(C)) return false;
                    if (ps[3].ParameterType != typeof(D)) return false;
                    return ctorPred(x);
                })
                .FirstOrDefault();

            if (ctorInfo == null) throw new ArgumentException($"Constructor not found for type {typeof(R).FullName}");

            var ctorParams = ctorInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            ctorInfo.DeclaringType,
                                            ctorParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, B, C, D, R>)dynamic.CreateDelegate(typeof(Func<A, B, C, D, R>));
        }

        /// <summary>
        /// Emits the IL to invoke a static method
        /// </summary>
        public static Option<Func<object, R>> Func1<TYPE, R>(Type arg1, Func<MethodInfo, bool> methodPred = null)
        {
            methodPred = methodPred ?? (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != arg1) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => typeof(object)).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.DeclareLocal(typeof(R));
            il.Emit(OpCodes.Ldarg_0);
            if (arg1.GetTypeInfo().IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, arg1);
            }
            else
            {
                il.Emit(OpCodes.Castclass, arg1);
            }
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<object, R>)dynamic.CreateDelegate(typeof(Func<object, R>));
        }

        /// <summary>
        /// Emits the IL to invoke a static method
        /// </summary>
        public static Option<Func<A, R>> Func1<TYPE, A, R>(Func<MethodInfo, bool> methodPred = null)
        {
            methodPred = methodPred ?? (_ => true);

            var methodInfo = typeof(TYPE)
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x =>
                {
                    if (!x.IsStatic) return false;
                    var ps = x.GetParameters();
                    if (ps.Length != 1) return false;
                    if (ps[0].ParameterType != typeof(A)) return false;
                    return methodPred(x);
                })
                .FirstOrDefault();

            if (methodInfo == null) return None;

            var methodParams = methodInfo.GetParameters();

            var dynamic = new DynamicMethod("CreateInstance",
                                            typeof(R),
                                            methodParams.Select(p => p.ParameterType).ToArray(),
                                            true);

            var il = dynamic.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return (Func<A, R>)dynamic.CreateDelegate(typeof(Func<A, R>));
        }
    }
}
