#nullable enable
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Lifts a lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Many<A> many<A>(IEnumerable<A> items) =>
        Many<A>.From(items);

    /// <summary>
    /// Lifts a lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Many<A> many<A>(Seq<A> items) =>
        Many<A>.From(items);

    /// <summary>
    /// Lifts an asynchronous lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Many<A> many<A>(IAsyncEnumerable<A> items) =>
        Many<A>.From(items);
}
