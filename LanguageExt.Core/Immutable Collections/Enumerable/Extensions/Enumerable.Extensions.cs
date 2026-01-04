#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Text;

namespace LanguageExt;

public static partial class EnumerableExtensions
{
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<A>(IEnumerable<A> ma)
    {
        /// <summary>
        /// Monadic bind function for IEnumerable
        /// </summary>
        [Pure]
        public IEnumerable<R> Bind<R>(Func<A, IEnumerable<R>> binder) =>
            ma.BindFast(binder);

        [Pure]
        public IEnumerable<A> Init() =>
            Iterable.init(ma);

        [Pure]
        public Iterable<A> Tail() =>
            Iterable.tail(ma);
        
        /// <summary>
        /// Force evaluation of the enumerable
        /// </summary>
        public Unit Consume() =>
            Iterable.consume(ma);

        /// <summary>
        /// Inject a value in between each item in the enumerable 
        /// </summary>
        /// <param name="value">Item to inject</param>
        /// <returns>An enumerable with the values injected</returns>
        [Pure]
        public Iterable<A> Intersperse(A value)
        {
            return go().AsIterable();
            IEnumerable<A> go()
            {
                var isFirst = true;
                foreach (var item in ma)
                {
                    if (!isFirst)
                    {
                        yield return value;
                    }

                    yield return item;
                    isFirst = false;
                }
            }
        }
    }

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Concatenate all strings into one
    /// </summary>
    [Pure]
    public static string Concat(this IEnumerable<string> xs)
    {
        var sb = new StringBuilder();
        foreach (var x in xs)
        {
            sb.Append(x);
        }

        return sb.ToString();
    }
}
