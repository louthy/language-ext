using System;
using System.Reflection.Emit;

namespace LanguageExt
{
    /// <summary>
    /// Represents a boxed value, if and only if `A` is a `struct`.  This can be used
    /// to stop unnecessary allocations when using generic types cast to `object`: the 
    /// Box isn't allocated for reference types and is for value-types.  Then access
    /// is transparent via `GetValue`.
    /// </summary>
    internal class Box<A>
    {
        public readonly A Value;

        public Box(A value) =>
            Value = value;

        public static readonly Func<A, object> New;
        public static readonly Func<object, A> GetValue;

        static Box()
        {
            New = typeof(A).IsValueType
                ? MakeNewStruct()
                : MakeNewClass();

            GetValue = typeof(A).IsValueType
                ? GetValueStruct()
                : GetValueClass();
        }

        static Func<object, A> GetValueClass()
        {
            if (ILCapability.Available)
            {
                var dynamic = new DynamicMethod("GetValue_Class", typeof(A), new[] {typeof(object)}, typeof(A).Module, true);
                var il      = dynamic.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);

                return (Func<object, A>)dynamic.CreateDelegate(typeof(Func<object, A>));
            }
            else
            {
                return (object x) => (A)x;
            }
        }

        static Func<object, A> GetValueStruct()
        {
            if (ILCapability.Available)
            {
                var field   = typeof(Box<A>).GetField("Value");
                var dynamic = new DynamicMethod("GetValue_Struct", typeof(A), new[] { typeof(object) }, typeof(A).Module, true);
                var il      = dynamic.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, typeof(Box<A>));
                il.Emit(OpCodes.Ldfld, field);
                il.Emit(OpCodes.Ret);

                return (Func<object, A>)dynamic.CreateDelegate(typeof(Func<object, A>));
            }
            else
            {
                return (object x) => ((Box<A>)x).Value;
            }
        }

        static Func<A, object> MakeNewClass()
        {
            if (ILCapability.Available)
            {
                var dynamic = new DynamicMethod("New_Class", typeof(object), new[] {typeof(A)}, typeof(A).Module, true);
                var il      = dynamic.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);

                return (Func<A, object>)dynamic.CreateDelegate(typeof(Func<A, object>));
            }
            else
            {
                return static (A x) => (object)x;
            }
        }

        static Func<A, object> MakeNewStruct()
        {
            if (ILCapability.Available)
            {
                var ctor    = typeof(Box<A>).GetConstructor(new[] {typeof(A)});
                var dynamic = new DynamicMethod("New_Struct", typeof(object), new[] {typeof(A)}, typeof(A).Module, true);
                var il      = dynamic.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);

                return (Func<A, object>)dynamic.CreateDelegate(typeof(Func<A, object>));
            }
            else
            {
                return static (A x) => new Box<A>(x);
            }
        }
    }
}
