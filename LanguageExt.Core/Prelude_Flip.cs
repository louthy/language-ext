using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Reverse the order of the first two arguments of a curried function
        /// </summary>
        [Pure]
        public static Func<T2, Func<T1, TResult>> flip<T1, T2, TResult>(this Func<T1, Func<T2, TResult>> function)
        {
            return arg2 => arg1 => function(arg1)(arg2);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, TResult> flip<T1, T2, TResult>(this Func<T1, T2, TResult> function)
        {
            return (arg2, arg1) => function(arg1, arg2);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, TResult> flip<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> function)
        {
            return (arg2, arg1, arg3) => function(arg1, arg2, arg3);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, TResult> flip<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function)
        {
            return (arg2, arg1, arg3, arg4) => function(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, TResult> flip<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5) => function(arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, TResult> flip<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6) => function(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, TResult> flip<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary>
        /// Reverse the order of the first two arguments of a function
        /// </summary>
        [Pure]
        public static Func<T2, T1, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> flip<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> function)
        {
            return (arg2, arg1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16) => function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }
    }
}