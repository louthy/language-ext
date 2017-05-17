using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class ComposeExtensions
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>v => b(a(v))</returns>
        [Pure]
        public static Func<T1, T3> BackCompose<T1, T2, T3>(this Func<T2, T3> b, Func<T1, T2> a) =>
            v => b(a(v));

        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>() => b(a())</returns>
        [Pure]
        public static Func<T2> BackCompose<T1, T2>(this Func<T1, T2> b, Func<T1> a) =>
            () => b(a());

        /// <summary>
        /// Action composition
        /// </summary>
        /// <returns>v => b(a(v))</returns>
        [Pure]
        public static Action<T1> BackCompose<T1, T2>(this Action<T2> b, Func<T1, T2> a)
            => v => b(a(v));

        /// <summary>
        /// Action composition
        /// </summary>
        /// <returns>() => b(a())</returns>
        [Pure]
        public static Action BackCompose<T>(this Action<T> b, Func<T> a)
            => () => b(a());

        /// <summary>
        /// Function back composition
        /// </summary>
        /// <returns>v => b(a(v))</returns>
        [Pure]
        public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T1, T2> a, Func<T2, T3> b) =>
            v => b(a(v));

        /// <summary>
        /// Function back composition
        /// </summary>
        /// <returns>() => b(a())</returns>
        [Pure]
        public static Func<T2> Compose<T1, T2>(this Func<T1> a, Func<T1, T2> b) =>
            () => b(a());

        /// <summary>
        /// Action back composition
        /// </summary>
        /// <returns>v => b(a(v))</returns>
        [Pure]
        public static Action<T1> Compose<T1, T2>(this Func<T1, T2> a, Action<T2> b)
            => v => b(a(v));

        /// <summary>
        /// Action back composition
        /// </summary>
        /// <returns>() => b(a())</returns>
        [Pure]
        public static Action Compose<T>(this Func<T> a, Action<T> b)
            => () => b(a());
    }
}
