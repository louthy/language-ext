using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class __ComposeExt
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <returns>b(a(v))</returns>
        [Pure]
        public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T2, T3> b, Func<T1, T2> a) =>
            v => b(a(v));

        /// <summary>
        /// Function back composition
        /// </summary>
        /// <returns>b(a(v))</returns>
        [Pure]
        public static Func<T1, T3> BackCompose<T1, T2, T3>(this Func<T1, T2> a, Func<T2, T3> b) =>
            v => b(a(v));
    }
}
