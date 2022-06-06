#nullable enable

using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Singleton source of randomness.
    /// </summary>
    internal static class SingletonRandom
    {
        static readonly Random Random = new();
        static readonly Func<int, Random> Provider = memo((int seed) => new Random(seed));

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0,
        /// and less than 1.0.
        /// </summary>
        internal static double NextDouble(Option<int> seed = default)
        {
            lock (Provider) return seed.Match(Provider, () => Random).NextDouble();
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to <paramref name="a"/>,
        /// and less than <paramref name="b"/>.
        /// </summary>
        internal static double Uniform(double a, double b, Option<int> seed = default)
        {
            if (a.Equals(b)) return a;
            return a + (b - a) * NextDouble(seed);
        }
    }
}
