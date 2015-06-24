using System;

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
        static Func<T1, Func<T2, R>> curry<T1, T2, R>(Func<T1, T2, R> f) =>
            (T1 a) => (T2 b) => f(a, b);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)
        /// 
        /// </summary>
        static Func<T1, Func<T2, Func<T3, R>>> curry<T1, T2, T3, R>(Func<T1, T2, T3, R> f) =>
            (T1 a) => (T2 b) => (T3 c) => f(a, b, c);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)
        /// 
        /// </summary>
        static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> curry<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => f(a, b, c, d);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)
        /// 
        /// </summary>
        static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> curry<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => f(a, b, c, d, e);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)
        /// 
        /// </summary>
        static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, R>>>>>> curry<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => func(a, b, c, d, e, f);


        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling: 
        /// 
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c)(d)(e)(f)(g)
        /// 
        /// </summary>
        static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, R>>>>>>> curry<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
            (T1 a) => (T2 b) => (T3 c) => (T4 d) => (T5 e) => (T6 f) => (T7 g) => func(a, b, c, d, e, f, g);
    }
}
