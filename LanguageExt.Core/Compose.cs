using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static class ComposeExtensions
    {
        /// <summary>
        /// Function back composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Func<T1, T3> BackCompose<T1, T2, T3>(this Func<T2, T3> g, Func<T1, T2> f) =>
            v => g(f(v));

        /// <summary>
        /// Function back composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Func<T2> BackCompose<T1, T2>(this Func<T1, T2> g, Func<T1> f) =>
            () => g(f());

        /// <summary>
        /// Action back composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Action<T1> BackCompose<T1, T2>(this Action<T2> g, Func<T1, T2> f)
            => v => g(f(v));

        /// <summary>
        /// Action back composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Action BackCompose<T>(this Action<T> g, Func<T> f)
            => () => g(f());

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T1, T2> f, Func<T2, T3> g) =>
            v => g(f(v));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Func<T2> Compose<T1, T2>(this Func<T1> f, Func<T1, T2> g) =>
            () => g(f());

        /// <summary>
        /// Action composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Action<T1> Compose<T1, T2>(this Func<T1, T2> f, Action<T2> g)
            => v => g(f(v));

        /// <summary>
        /// Action composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Action Compose<T>(this Func<T> f, Action<T> g)
            => () => g(f());
    }
}
