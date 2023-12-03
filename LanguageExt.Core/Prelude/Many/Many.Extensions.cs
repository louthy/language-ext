#nullable enable
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using LanguageExt.Transducers;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class ManyExtensions
{
    /// <summary>
    /// Lifts a lazy sequence into a transducer that streams the values
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<Unit, A> Many<A>(this IEnumerable<A> items) =>
        many(items);

    /// <summary>
    /// Lifts a lazy sequence into a transducer that streams the values
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<Unit, A> Many<A>(this Seq<A> items) =>
        many(items);

    /// <summary>
    /// Lifts an asynchronous lazy sequence into a transducer that streams the values
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<Unit, A> Many<A>(this IAsyncEnumerable<A> items) =>
        many(items);
}
