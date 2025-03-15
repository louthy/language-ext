using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

public static class SourceTExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static SourceT<M, A> As<M, A>(this K<SourceT<M>, A> ma) 
        where M : Monad<M>, Alternative<M> =>
        (SourceT<M, A>)ma;

    /// <summary>
    /// Force iteration of the stream, yielding a unit `M` structure.
    /// </summary>
    /// <remarks>
    /// The expectation is that the stream uses `IO` for side effects, so this makes them to happen.
    /// </remarks>
    public static K<M, Unit> Iter<M, A>(this K<SourceT<M>, A> ma)
        where M : Monad<M>, Alternative<M> =>
        ma.As().Reduce(unit, (_, _) => M.Pure(unit));

    /// <summary>
    /// Force iteration of the stream, yielding the last structure processed
    /// </summary>
    public static K<M, A> Last<M, A>(this K<SourceT<M>, A> ma)
        where M : Monad<M>, Alternative<M> =>
        ma.As()
          .Reduce(Option<A>.None, (_, x) => M.Pure(Some(x)))
          .Bind(ma => ma switch
                      {
                          { IsSome: true, Case: A value } => M.Pure(value),
                          _                               => M.Empty<A>()
                      });

    /// <summary>
    /// Force iteration of the stream and collect all the values into a `Seq`.
    /// </summary>
    public static K<M, Seq<A>> Collect<M, A>(this K<SourceT<M>, A> ma)
        where M : Monad<M>, Alternative<M> =>
        ma.As().Reduce<Seq<A>>([], (xs, x) => M.Pure(xs.Add(x)));
    
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static SourceT<M, C> SelectMany<M, A, B, C>(this IO<A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : Monad<M>, Alternative<M> =>
        SourceT.liftIO<M, A>(ma).As().SelectMany(bind, project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static SourceT<M, C> SelectMany<M, A, B, C>(this Pure<A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : Monad<M>, Alternative<M> =>
        bind(ma.Value).Map(y => project(ma.Value, y));
}
