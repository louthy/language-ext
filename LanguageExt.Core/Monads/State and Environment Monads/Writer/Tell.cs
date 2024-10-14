using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// State put
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `put` function.  It avoids
/// the need for lots of generic parameters when used in `WriterT` and `State` based
/// monads.
/// </remarks>
/// <param name="F">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public record Tell<W>(W Value) where W : Monoid<W>
{
    /// <summary>
    /// Convert ot a `WriterT`
    /// </summary>
    public WriterT<W, M, Unit> ToWriterT<M>()
        where M : Monad<M>, SemigroupK<M> =>
        new (w => M.Pure((unit, w + Value)));
    
    /// <summary>
    /// Convert ot a `WriterT`
    /// </summary>
    public Writer<W, Unit> ToWriter() =>
        new (w => (unit, w + Value));
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    //public State<S, Unit> ToState() =>
    //    State<S, Unit>.Put(Value).As();

    /// <summary>
    /// Monadic bind with `WriterT`
    /// </summary>
    public WriterT<W, M, C> SelectMany<M, B, C>(Func<Unit, WriterT<W, M, B>> bind, Func<Unit, B, C> project)
        where M : Monad<M>, SemigroupK<M> =>
        ToWriterT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `State`
    /// </summary>
    public Writer<W, C> SelectMany<B, C>(Func<Unit, Writer<W, B>> bind, Func<Unit, B, C> project) =>
        ToWriter().SelectMany(bind, project);
}
