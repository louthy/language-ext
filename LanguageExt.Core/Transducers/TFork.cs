using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Transducers;

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
        where RT : struct, HasIO<RT, E> =>
        new (IO<RT, E, Unit>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), fork.Cancel)),
             IO<RT, E, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), fork.Await)));
    
}
