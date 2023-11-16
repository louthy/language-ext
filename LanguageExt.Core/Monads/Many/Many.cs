#nullable enable
using System;
using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt;

/// <summary>
/// Represents many pure values.
/// </summary>
/// <remarks>
/// When used with transducers, or transducer-based monads, it turns them into
/// stream processes rather than single item processes.  
/// </summary>
/// <remarks>
/// On its own this doesn't do much, but  allows other monads to convert
/// from it and provide binding extensions that mean it can be lifted into
/// other monads without specifying lots of extra generic arguments.
/// </remarks>
/// <param name="Value">Bound value</param>
/// <typeparam name="A">Bound value type</typeparam>
public readonly struct Many<A> : Transducer<Unit, A>
{
    readonly Transducer<Unit, A>? items;
    Transducer<Unit, A> Items => items ?? Transducer.Fail<Unit, A>(Errors.Bottom);
    
    internal Many(Transducer<Unit, A> items) =>
        this.items = items;

    /// <summary>
    /// Create a new `Many` from an enumerable 
    /// </summary>
    /// <param name="items">Items to stream</param>
    /// <returns>Transducer that encapsulates the streaming</returns>
    public static Many<A> From(IEnumerable<A> items) =>
        new(Transducer.compose(Transducer.constant<Unit, IEnumerable<A>>(items), Transducer.enumerable<A>()));

    /// <summary>
    /// Create a new `Many` from an enumerable 
    /// </summary>
    /// <param name="items">Items to stream</param>
    /// <returns>Transducer that encapsulates the streaming</returns>
    public static Many<A> From(Seq<A> items) =>
        new(Transducer.compose(Transducer.constant<Unit, Seq<A>>(items), Transducer.seq<A>()));

    /// <summary>
    /// Create a new `Many` from an asynchronous enumerable 
    /// </summary>
    /// <param name="items">Items to stream</param>
    /// <returns>Transducer that encapsulates the streaming</returns>
    public static Many<A> From(IAsyncEnumerable<A> items) =>
        new(Transducer.compose(Transducer.constant<Unit, IAsyncEnumerable<A>>(items), Transducer.asyncEnumerable<A>()));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Standard operators
    //

    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the mapping function to the items</returns>
    public Many<B> Map<B>(Func<A, B> f) =>
        new (Items.Map(f));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the bind function to the items</returns>
    public Many<B> Bind<B>(Func<A, Many<B>> f) =>
        new(Items.Bind(x => f(x).Morphism));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <typeparam name="B">Result of the bind operation bound value type</typeparam>
    /// <typeparam name="C">Result of the mapping operation bound value type</typeparam>
    /// <returns>Result of the applying the bind and mapping function to the items</returns>
    public Many<C> SelectMany<B, C>(Func<A, Many<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public IO<RT, E, A> ToIO<RT, E>() where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), Items));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public IO<RT, E, B> Bind<RT, E, B>(Func<A, IO<RT, E, B>> bind)
        where RT : struct, HasIO<RT, E> =>
        ToIO<RT, E>().Bind(bind);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public IO<RT, E, C> SelectMany<RT, E, B, C>(Func<A, IO<RT, E, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        ToIO<RT, E>().SelectMany(bind, project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Transducer
    //

    public Transducer<Unit, A> Morphism =>
        Items;

    public Reducer<Unit, S> Transform<S>(Reducer<A, S> reduce) => 
        Items.Transform(reduce);
}
