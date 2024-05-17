using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, R> Uncurry<T1, T2, R>(this Func<T1, Func<T2, R>> function) =>
        (arg1, arg2) => function(arg1)(arg2);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, R> Uncurry<T1, T2, T3, R>(this Func<T1, Func<T2, Func<T3, R>>> function) =>
        (arg1, arg2, arg3) => function(arg1)(arg2)(arg3);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, R> Uncurry<T1, T2, T3, T4, R>(this Func<T1, Func<T2, Func<T3, Func<T4, R>>>> function) =>
        (arg1, arg2, arg3, arg4) => function(arg1)(arg2)(arg3)(arg4);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, R> Uncurry<T1, T2, T3, T4, T5, R>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5) => function(arg1)(arg2)(arg3)(arg4)(arg5);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, R> Uncurry<T1, T2, T3, T4, T5, T6, R>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5, arg6) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, R> Uncurry<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, R> Uncurry<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, R>>>>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8);


    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, R> uncurry<T1, T2, R>(Func<T1, Func<T2, R>> function) =>
        (arg1, arg2) => function(arg1)(arg2);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, R> uncurry<T1, T2, T3, R>(Func<T1, Func<T2, Func<T3, R>>> function) =>
        (arg1, arg2, arg3) => function(arg1)(arg2)(arg3);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, R> uncurry<T1, T2, T3, T4, R>(Func<T1, Func<T2, Func<T3, Func<T4, R>>>> function) =>
        (arg1, arg2, arg3, arg4) => function(arg1)(arg2)(arg3)(arg4);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, R> uncurry<T1, T2, T3, T4, T5, R>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5) => function(arg1)(arg2)(arg3)(arg4)(arg5);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, R> uncurry<T1, T2, T3, T4, T5, T6, R>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5, arg6) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, R> uncurry<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7);

    /// <summary>
    /// Transforms a curried function into a function that takes multiple arguments
    /// </summary>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, R> uncurry<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, R>>>>>>>> function) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => function(arg1)(arg2)(arg3)(arg4)(arg5)(arg6)(arg7)(arg8);

}
