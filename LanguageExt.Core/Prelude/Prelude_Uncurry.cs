using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, TResult> uncurry<T1, T2, TResult>(this Func<T1, Func<T2, TResult>> function) =>
            (arg1, arg2) => function(arg1)(arg2);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, TResult> uncurry<T1, T2, T3, TResult>(this Func<T1, Func<T2, Func<T3, TResult>>> function) =>
            (arg1, arg2, arg3) => function(arg1)(arg2)(arg3);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, TResult> uncurry<T1, T2, T3, T4, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> function) =>
            (arg1, arg2, arg3, arg4) => function(arg1)(arg2)(arg3)(arg4);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, TResult> uncurry<T1, T2, T3, T4, T5, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5) => function(arg1)(arg2)(arg3)(arg4)(arg5);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> uncurry<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, TResult>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, TResult>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, Func<T11, TResult>>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10)(arg11);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, Func<T11, Func<T12, TResult>>>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10)(arg11)(arg12);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, Func<T11, Func<T12, Func<T13, TResult>>>>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10)(arg11)(arg12)(arg13);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, Func<T11, Func<T12, Func<T13, Func<T14, TResult>>>>>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10)(arg11)(arg12)(arg13)(arg14);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, Func<T11, Func<T12, Func<T13, Func<T14, Func<T15, TResult>>>>>>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10)(arg11)(arg12)(arg13)(arg14)(arg15);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, Func<T11, Func<T12, Func<T13, Func<T14, Func<T15, Func<T16, TResult>>>>>>>>>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8)(arg9)(arg10)(arg11)(arg12)(arg13)(arg14)(arg15)(arg16);
    }
}