using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Left partially apply 
    /// </summary>
    [Pure]
    public static Func<T1, R> lpar<T1, T2, R>(Func<T1, T2, R> func, T2 b) =>
        a => func(a, b);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, R> par<T1, T2, R>(Func<T1, T2, R> func, T1 a) =>
        b => func(a, b);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, R> par<T1, T2, T3, R>(Func<T1, T2, T3, R> func, T1 a, T2 b) =>
        c => func(a, b, c);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, R> par<T1, T2, T3, R>(Func<T1, T2, T3, R> func, T1 a) =>
        (b, c) => func(a, b, c);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, R> par<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, T1 a, T2 b, T3 c) =>
        d => func(a, b, c, d);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, R> par<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, T1 a, T2 b) =>
        (c, d) => func(a, b, c, d);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, R> par<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, T1 a) =>
        (b, c, d) => func(a, b, c, d);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T5, R> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a, T2 b, T3 c, T4 d) =>
        e => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, T5, R> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a, T2 b, T3 c) =>
        (d, e) => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, T5, R> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a, T2 b) =>
        (c, d, e) => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, T5, R> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a) =>
        (b, c, d, e) => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T6, R> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        f => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T5, T6, R> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, T5, T6, R> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b, T3 c) =>
        (d, e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, T5, T6, R> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b) =>
        (c, d, e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, T5, T6, R> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a) =>
        (b, c, d, e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        g => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T6, T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T5, T6, T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, T5, T6, T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, T5, T6, T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b) =>
        (c, d, e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, T5, T6, T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a) =>
        (b, c, d, e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, T5, T6, T7, T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a) =>
        (b, c, d, e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, T5, T6, T7, T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a, T2 b) =>
        (c, d, e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, T5, T6, T7, T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T5, T6, T7, T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T6, T7, T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T7, T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        (g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T8, R> par<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g) =>
        h => func(a, b, c, d, e, f, g, h);


    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, T5, T6, T7, T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a) =>
        (b, c, d, e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, T5, T6, T7, T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b) =>
        (c, d, e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, T5, T6, T7, T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T5, T6, T7, T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T6, T7, T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T7, T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        (g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T8, T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g) =>
        (h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T9, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h) =>
        i => func(a, b, c, d, e, f, g, h, i);


    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a) =>
        (b, c, d, e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T3, T4, T5, T6, T7, T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b) =>
        (c, d, e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T4, T5, T6, T7, T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T5, T6, T7, T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T6, T7, T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T7, T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        (g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T8, T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g) =>
        (h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T9, T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h) =>
        (i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Func<T10, R> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i) =>
        j => func(a, b, c, d, e, f, g, h, i, j);





    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2> par<T1, T2>(Action<T1, T2> func, T1 a) =>
        b => func(a, b);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3> par<T1, T2, T3>(Action<T1, T2, T3> func, T1 a, T2 b) =>
        c => func(a, b, c);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3> par<T1, T2, T3>(Action<T1, T2, T3> func, T1 a) =>
        (b, c) => func(a, b, c);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4> par<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 a, T2 b, T3 c) =>
        d => func(a, b, c, d);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4> par<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 a, T2 b) =>
        (c, d) => func(a, b, c, d);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4> par<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 a) =>
        (b, c, d) => func(a, b, c, d);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T5> par<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 a, T2 b, T3 c, T4 d) =>
        e => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4, T5> par<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 a, T2 b, T3 c) =>
        (d, e) => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4, T5> par<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 a, T2 b) =>
        (c, d, e) => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4, T5> par<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 a) =>
        (b, c, d, e) => func(a, b, c, d, e);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T6> par<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        f => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T5, T6> par<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4, T5, T6> par<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 a, T2 b, T3 c) =>
        (d, e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4, T5, T6> par<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 a, T2 b) =>
        (c, d, e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4, T5, T6> par<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 a) =>
        (b, c, d, e, f) => func(a, b, c, d, e, f);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T7> par<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        g => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T6, T7> par<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T5, T6, T7> par<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4, T5, T6, T7> par<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4, T5, T6, T7> par<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 a, T2 b) =>
        (c, d, e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4, T5, T6, T7> par<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 a) =>
        (b, c, d, e, f, g) => func(a, b, c, d, e, f, g);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4, T5, T6, T7, T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a) =>
        (b, c, d, e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4, T5, T6, T7, T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a, T2 b) =>
        (c, d, e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4, T5, T6, T7, T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T5, T6, T7, T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T6, T7, T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T7, T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        (g, h) => func(a, b, c, d, e, f, g, h);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T8> par<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g) =>
        h => func(a, b, c, d, e, f, g, h);


    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4, T5, T6, T7, T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a) =>
        (b, c, d, e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4, T5, T6, T7, T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b) =>
        (c, d, e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4, T5, T6, T7, T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T5, T6, T7, T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T6, T7, T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T7, T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        (g, h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T8, T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g) =>
        (h, i) => func(a, b, c, d, e, f, g, h, i);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T9> par<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h) =>
        i => func(a, b, c, d, e, f, g, h, i);


    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T2, T3, T4, T5, T6, T7, T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a) =>
        (b, c, d, e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T3, T4, T5, T6, T7, T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b) =>
        (c, d, e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T4, T5, T6, T7, T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c) =>
        (d, e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T5, T6, T7, T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c, T4 d) =>
        (e, f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T6, T7, T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
        (f, g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T7, T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
        (g, h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T8, T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g) =>
        (h, i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T9, T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h) =>
        (i, j) => func(a, b, c, d, e, f, g, h, i, j);

    /// <summary>
    /// Partially apply 
    /// </summary>
    [Pure]
    public static Action<T10> par<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i) =>
        j => func(a, b, c, d, e, f, g, h, i, j);
}
