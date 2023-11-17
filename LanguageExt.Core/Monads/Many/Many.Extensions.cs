#nullable enable
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class ManyExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    /// <param name="mma">Nested `Pure` monad</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Flattened monad</returns>
    public static Many<A> Flatten<A>(this Many<Many<A>> mma) =>
        mma.Bind(identity);
    
    /// <summary>
    /// Lifts a lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Many<A> Many<A>(this IEnumerable<A> items) =>
        LanguageExt.Many<A>.From(items);

    /// <summary>
    /// Lifts a lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Many<A> Many<A>(this Seq<A> items) =>
        LanguageExt.Many<A>.From(items);

    /// <summary>
    /// Lifts an asynchronous lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Many<A> Many<A>(this IAsyncEnumerable<A> items) =>
        LanguageExt.Many<A>.From(items);
}
