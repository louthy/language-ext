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
        public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(this Func<T1, Func<T2, TResult>> function) =>
            (arg1, arg2) => function(arg1)(arg2);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, TResult> Uncurry<T1, T2, T3, TResult>(this Func<T1, Func<T2, Func<T3, TResult>>> function) =>
            (arg1, arg2, arg3) => function(arg1)(arg2)(arg3);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, TResult> Uncurry<T1, T2, T3, T4, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> function) =>
            (arg1, arg2, arg3, arg4) => function(arg1)(arg2)(arg3)(arg4);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, TResult> Uncurry<T1, T2, T3, T4, T5, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5) => function(arg1)(arg2)(arg3)(arg4)(arg5);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Uncurry<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Uncurry<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Uncurry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8);


        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, TResult> uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> function) =>
            (arg1, arg2) => function(arg1)(arg2);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, TResult> uncurry<T1, T2, T3, TResult>(Func<T1, Func<T2, Func<T3, TResult>>> function) =>
            (arg1, arg2, arg3) => function(arg1)(arg2)(arg3);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, TResult> uncurry<T1, T2, T3, T4, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> function) =>
            (arg1, arg2, arg3, arg4) => function(arg1)(arg2)(arg3)(arg4);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, TResult> uncurry<T1, T2, T3, T4, T5, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5) => function(arg1)(arg2)(arg3)(arg4)(arg5);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> uncurry<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, TResult>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, TResult>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7);

        /// <summary>
        /// Transforms a curried function into a function that takes multiple arguments
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, TResult>>>>>>>> function) =>
            (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8);

    }
}