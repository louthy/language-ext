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
        public static Func<A, C> BackCompose<A, B, C>(this Func<B, C> g, Func<A, B> f) =>
            v => g(f(v));

        /// <summary>
        /// Function back composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Func<B> BackCompose<A, B>(this Func<A, B> g, Func<A> f) =>
            () => g(f());

        /// <summary>
        /// Action back composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Action<A> BackCompose<A, B>(this Action<B> g, Func<A, B> f)
            => v => g(f(v));

        /// <summary>
        /// Action back composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Action BackCompose<A>(this Action<A> g, Func<A> f)
            => () => g(f());

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Func<A, C> Compose<A, B, C>(this Func<A, B> f, Func<B, C> g) =>
            v => g(f(v));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Func<B> Compose<A, B>(this Func<A> f, Func<A, B> g) =>
            () => g(f());

        /// <summary>
        /// Action composition
        /// </summary>
        /// <returns>v => g(f(v))</returns>
        [Pure]
        public static Action<A> Compose<A, B>(this Func<A, B> f, Action<B> g)
            => v => g(f(v));

        /// <summary>
        /// Action composition
        /// </summary>
        /// <returns>() => g(f())</returns>
        [Pure]
        public static Action Compose<A>(this Func<A> f, Action<A> g)
            => () => g(f());
    }
}
