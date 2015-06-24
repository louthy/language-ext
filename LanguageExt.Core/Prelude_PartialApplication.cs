using System;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T2, R> par<T1, T2, R>(Func<T1, T2, R> func, T1 a) =>
            (T2 b) => func(a, b);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T3, R> par<T1, T2, T3, R>(Func<T1, T2, T3, R> func, T1 a, T2 b) =>
            (T3 c) => func(a, b, c);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T2, Func<T3, R>> par<T1, T2, T3, R>(Func<T1, T2, T3, R> func, T1 a) =>
            (T2 b) => (T3 c) => func(a, b, c);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T4, R> par<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, T1 a, T2 b, T3 c) =>
            (T4 d) => func(a, b, c, d);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T3, Func<T4, R>> par<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, T1 a, T2 b) =>
            (T3 c) => (T4 d) => func(a, b, c, d);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T2, Func<T3, Func<T4, R>>> par<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, T1 a) =>
            (T2 b) => (T3 c) => (T4 d) => func(a, b, c, d);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T5, R> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a, T2 b, T3 c, T4 d) =>
            (T5 e) => func(a, b, c, d, e);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T4, Func<T5, R>> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a, T2 b, T3 c) =>
            (T4 d) => (T5 e) => func(a, b, c, d, e);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T3, Func<T4, Func<T5, R>>> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a, T2 b) =>
            (T3 c) => (T4 d) => (T5 e) => func(a, b, c, d, e);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T2, Func<T3, Func<T4, Func<T5, R>>>> par<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, T1 a) =>
            (T2 b) => (T3 c) => (T4 d) => (T5 e) => func(a, b, c, d, e);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T6, R> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
            (T6 f) => func(a, b, c, d, e, f);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T5, Func<T6, R>> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b, T3 c, T4 d) =>
            (T5 e) => (T6 f) => func(a, b, c, d, e, f);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T4, Func<T5, Func<T6, R>>> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b, T3 c) =>
            (T4 d) => (T5 e) => (T6 f) => func(a, b, c, d, e, f);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T3, Func<T4, Func<T5, Func<T6, R>>>> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a, T2 b) =>
            (T3 c) => (T4 d) => (T5 e) => (T6 f) => func(a, b, c, d, e, f);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>> par<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, T1 a) =>
            (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => func(a, b, c, d, e, f);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T7, R> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c, T4 d, T5 e, T6 f) =>
            (T7 g) => func(a, b, c, d, e, f, g);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T6, Func<T7, R>> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c, T4 d, T5 e) =>
            (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T5, Func<T6, Func<T7, R>>> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c, T4 d) =>
            (T5 e) => (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T4, Func<T5, Func<T6, Func<T7, R>>>> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b, T3 c) =>
            (T4 d) => (T5 e) => (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a, T2 b) =>
            (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);

        /// <summary>
        /// Partially apply 
        /// </summary>
        static Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>> par<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, T1 a) =>
            (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);

    }
}
