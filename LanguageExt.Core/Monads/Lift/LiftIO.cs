#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

/// <summary>
/// Represents a lifted IO function.  
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
/// <typeparam name="A">Bound value type</typeparam>
/// <param name="F">Lifted function</param>
public readonly record struct LiftIO<A>(Func<CancellationToken, Task<A>> F)
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
    /// <returns>Mapped lifted IO</returns>
    public LiftIO<B> Map<B>(Func<A, B> f)
    {
        var fn = F;
        return new(ct => fn(ct).Map(f));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //

    public IO<RT, E, A> ToIO<RT, E>()
        where RT : struct, HasIO<RT, E>
    {
        var fn = F;
        return IO<RT, E, A>.LiftIO(rt => fn(rt.CancellationToken));
    }

    public Eff<RT, A> ToEff<RT>()
        where RT : struct, HasIO<RT, Error> =>
        throw new NotImplementedException("TODO");
    
    public Eff<A> ToEff() =>
        throw new NotImplementedException("TODO");
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public IO<RT, E, B> Bind<RT, E, B>(Func<A, IO<RT, E, B>> bind)
        where RT : struct, HasIO<RT, E> =>
        ToIO<RT, E>().Bind(bind);

    public Eff<RT, B> Bind<RT, B>(Func<A, Eff<RT, B>> bind)
        where RT : struct, HasIO<RT, Error> =>
        ToEff<RT>().Bind(bind);

    public Eff<B> Bind<B>(Func<A, Eff<B>> bind) =>
        ToEff().Bind(bind);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public IO<RT, E, C> SelectMany<RT, E, B, C>(Func<A, IO<RT, E, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}
