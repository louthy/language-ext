using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

/// <summary>
/// Result of forking a transducer
/// </summary>
/// <param name="Cancel">A transducer, which if invoked, would cancel the forked transducer</param>
/// <param name="Await">A transducer, which if invoked, would attempt to get the result of the forked transducer</param>
/// <typeparam name="A"></typeparam>
public readonly record struct TFork<A>(
    Transducer<Unit, Unit> Cancel,
    Transducer<Unit, A> Await);

public static class TFork
{
    /// <summary>
    /// Convert a `TFork` to a `ForkIO`
    /// </summary>
    public static ForkIO<RT, E, A> ToIO<RT, E, A>(this TFork<Sum<E, A>> fork)
        where RT : HasIO<RT, E> =>
        new (IO<RT, E, Unit>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), fork.Cancel)),
             IO<RT, E, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), fork.Await)));
    
    /// <summary>
    /// Convert a `TFork` to a `ForkIO`
    /// </summary>
    public static ForkIO<E, A> ToIO<E, A>(this TFork<Sum<E, A>> fork) =>
        new (IO<E, Unit>.Lift(Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), fork.Cancel)),
             IO<E, A>.Lift(Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), fork.Await)));
    
    /// <summary>
    /// Convert a `TFork` to a `ForkEff`
    /// </summary>
    public static ForkEff<RT, A> ToEff<RT, A>(this TFork<Sum<Error, A>> fork)
        where RT : HasIO<RT, Error> =>
        new (Eff<RT, Unit>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), fork.Cancel)),
             Eff<RT, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), fork.Await)));
    
    /// <summary>
    /// Convert a `TFork` to a `ForkEff`
    /// </summary>
    public static ForkEff<A> ToEff<A>(this TFork<Sum<Error, A>> fork) =>
        new (Eff<Unit>.Lift(Transducer.compose(Transducer.constant<MinRT, Unit>(default), fork.Cancel)),
             Eff<A>.Lift(Transducer.compose(Transducer.constant<MinRT, Unit>(default), fork.Await)));
}
