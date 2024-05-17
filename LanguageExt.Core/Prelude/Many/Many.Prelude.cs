using System;
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
    public static Transducer<Unit, A> many<A>(IEnumerable<A> items) =>
        Transducer.compose(Transducer.constant<Unit, IEnumerable<A>>(items), Transducer.enumerable<A>());

    /// <summary>
    /// Lifts a lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<Unit, A> many<A>(Seq<A> items) =>
        Transducer.compose(Transducer.constant<Unit, Seq<A>>(items), Transducer.seq<A>());

    /// <summary>
    /// Lifts an asynchronous lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<Unit, A> many<A>(IAsyncEnumerable<A> items) =>
        Transducer.compose(Transducer.constant<Unit, IAsyncEnumerable<A>>(items), Transducer.asyncEnumerable<A>());

    /// <summary>
    /// Lifts an asynchronous lazy sequence into the `Many` monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`Many` monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<Unit, A> many<A>(IObservable<A> items) =>
        Transducer.compose(Transducer.constant<Unit, IObservable<A>>(items), Transducer.observable<A>());
}
