using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        public static int compare<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).Compare(x, y);

        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        public static int compare<ORD, A>(Optional<A> x, Optional<A> y) where ORD : struct, Ord<A> =>
            default(OrdOpt<ORD,A>).Compare(x, y);

        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        public static bool greaterThan<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).GreaterThan(x, y);

        /// <summary>
        /// Returns true if x is greater than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than or equal to y
        public static bool greaterOrEq<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).GreaterOrEq(x, y);

        /// <summary>
        /// Returns true if x is less than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than y
        public static bool lessThan<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).LessThan(x, y);

        /// <summary>
        /// Returns true if x is less than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than or equal to y
        public static bool lessOrEq<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).LessOrEq(x, y);
    }
}
