#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt;

/// <summary>
/// Represents a lifted function.  
/// </summary>
/// <remarks>
/// On its own this doesn't do much, but it allows other monads to convert
/// from it and provide binding extensions that mean this will work in
/// binding scenarios.
///
/// It simplifies certain scenarios where additional generic arguments are
/// needed.  This only requires the generic argument of the value which
/// means the C# inference system can work.
/// </remarks>
/// <typeparam name="B">Bound value type</typeparam>
/// <param name="F">Lifted function</param>
public readonly record struct Lift<A, B>(Func<A, B> F) : Transducer<A, B>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Standard operators
    //

    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Result of the mapping operation</typeparam>
    /// <returns>Mapped lifted-IO</returns>
    public Lift<A, C> Map<C>(Func<B, C> f)
    {
        var fn = F;
        return new(a => f(fn(a)));
    }

    /// <summary>
    /// Monad bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="B">Result of the bind operation</typeparam>
    /// <returns>Chained lifted-IO</returns>
    public Lift<A, C> Bind<C>(Func<B, Lift<A, C>> f)
    {
        var fn = F;
        return new(a => f(fn(a)).F(a));
    }

    /// <summary>
    /// Monad bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <typeparam name="B">Result of the bind operation</typeparam>
    /// <typeparam name="C">Result of the project operation</typeparam>
    /// <returns>Chained and mapped lifted-IO</returns>
    public Lift<A, D> SelectMany<C, D>(Func<B, Lift<A, C>> bind, Func<B, C, D> project)
    {
        var f = F;
        return new(a => f(a) switch
        {
            var b => project(b, bind(b).F(a))
        });
    }

    public Transducer<A, B> Morphism =>
        Transducer.lift(F);

    public Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        Morphism.Transform(reduce);
        
    public override string ToString() =>
        "lift";
}

public static class LiftExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    /// <param name="mma">Nested `Lift` monad</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Flattened monad</returns>
    public static Lift<A, B> Flatten<A, B>(this Lift<A, Lift<A, B>> mma)
    {
        var f = mma.F;
        return new(a => f(a).F(a));
    }
    /// <summary>
    /// Monadic join
    /// </summary>
    /// <param name="mma">Nested `Lift` monad</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Flattened monad</returns>
    public static Lift<A, B> Flatten<A, B>(this Lift<A, Lift<Unit, B>> mma)
    {
        var f = mma.F;
        return new(a => f(a).F(default));
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public static IO<RT, E, B> Bind<RT, E, A, B>(this Lift<RT, A> ma, Func<A, IO<RT, E, B>> bind)
        where RT : struct, HasIO<RT, E> =>
        ma.ToIO<RT, E, A>().Bind(bind);
        
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public static IO<RT, E, C> SelectMany<RT, E, A, B, C>(this Lift<RT, A> ma, Func<A, IO<RT, E, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        ma.ToIO<RT, E, A>().Bind(x => bind(x).Map(y => project(x, y)));

    public static IO<RT, E, C> SelectMany<RT, E, A, B, C>(this Lift<Unit, A> ma, Func<A, IO<RT, E, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(_ => ma.F(default)).Bind(x => bind(x).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //

    public static IO<RT, E, A> ToIO<RT, E, A>(this Lift<RT, A> ma)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(ma.F);
}
