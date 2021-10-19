using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class ValueTasks
    {
        [Pure]
        public static async ValueTask<bool> ForAll<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred)
        {
            var ra = await fs.WindowMap(pred).ConfigureAwait(false);
            return ra.ForAll(identity);
        }

        [Pure]
        public static async ValueTask<bool> ForAll<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred, int windowSize)
        {
            var ra = await fs.WindowMap(windowSize, pred).ConfigureAwait(false);
            return ra.ForAll(identity);
        }

        [Pure]
        public static async ValueTask<bool> Exists<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred)
        {
            var ra = await fs.WindowMap(pred).ConfigureAwait(false);
            return ra.Exists(identity);
        }

        [Pure]
        public static async ValueTask<bool> Exists<A>(IEnumerable<ValueTask<A>> fs, Func<A, bool> pred, int windowSize)
        {
            var ra = await fs.WindowMap(windowSize, pred).ConfigureAwait(false);
            return ra.Exists(identity);
        }    
    }
}
