using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.UnsafeValueAccess;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class EnumerableExtensions
{
    extension(Enumerable)
    {
        /// <summary>
        /// Unfolds or generates a sequence by iteratively applying a function to a seed value.
        /// The unfolding continues until the folder function returns None, signaling the end of the sequence.
        /// 
        /// <example>
        /// Creates a Range function that generates numbers from start to end (inclusive):
        /// <code>
        /// // Define a Range function using Unfold
        /// public static IEnumerable&lt;int&gt; Range(int start, int end) =>
        ///     Unfold(
        ///         seed => seed &lt;= end 
        ///             ? Some((seed, seed + 1))
        ///             : None,
        ///         start);
        /// 
        /// // Usage
        /// var numbers = Range(1, 5);  // Generates: 1, 2, 3, 4, 5
        /// foreach (var n in numbers)
        ///     Console.WriteLine(n);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TSeed">The type of the seed value used to generate the sequence</typeparam>
        /// <typeparam name="TResult">The type of elements in the resulting sequence</typeparam>
        /// <param name="folder">A function that takes a seed value and returns an Option containing either:
        /// - Some((value, nextSeed)) to yield a value and continue with the next seed, or
        /// - None to terminate the sequence</param>
        /// <param name="seed">The initial seed value to start the unfolding process</param>
        /// <returns>An IEnumerable that lazily generates the sequence through unfolding</returns>
        [Pure]
        public static IEnumerable<TResult> Unfold<TSeed, TResult>(Func<TSeed, Option<(TResult, TSeed)>> folder, TSeed seed)
        {
            while (true)
            {
                var result = folder(seed);
                if (result.IsNone) break;
                (var o, seed) = result.ValueUnsafe();
                yield return o;
            }
        }
    }
}
