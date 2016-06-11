using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, R>> curry<T1, T2, R>(Func<T1, T2, R> f) =>
            (T1 a) => (T2 b) => f(a, b);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, R>>> curry<T1, T2, T3, R>(Func<T1, T2, T3, R> f) =>
            (T1 a) => (T2 b) => (T3 c) => f(a, b, c);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> curry<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => f(a, b, c, d);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> curry<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => f(a, b, c, d, e);

        /// <summary>
        /// Curry the function 'func' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>>> curry<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => func(a, b, c, d, e, f);


        /// <summary>
        /// Curry the function 'func' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)(g)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>>> curry<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);


        /// <summary>
        /// Curry the function 'func' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)(g)(h)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, R>>>>>>>> curry<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => (T8 h) => func(a, b, c, d, e, f, g, h);

        /// <summary>
        /// Curry the function 'func' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)(g)(h)(i)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, R>>>>>>>>> curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => (T8 h) => (T9 i) => func(a, b, c, d, e, f, g, h, i);

        /// <summary>
        /// Curry the function 'func' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)(g)(h)(i)(j)
        /// 
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, Func<T10, R>>>>>>>>>> curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => (T8 h) => (T9 i) => (T10 j) => func(a, b, c, d, e, f, g, h, i, j);
    }
}
