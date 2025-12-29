using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SourceExtensions
{
    /// <summary>
    /// Force iteration of the stream, yielding a unit `M` structure.
    /// </summary>
    /// <remarks>
    /// The expectation is that the stream uses `IO` for side effects, so this makes them to happen.
    /// </remarks>
    public static IO<Unit> Iter<A>(this K<Source, A> ma) =>
        ma.As().ReduceIO(unit, (_, _) => Reduced.ContinueIO(unit));
    
    /// <summary>
    /// Force iteration of the stream, yielding a unit `M` structure.
    /// </summary>
    /// <remarks>
    /// The expectation is that the stream uses `IO` for side effects, so this makes them to happen.
    /// </remarks>
    public static K<M, Unit> Iter<M, A>(this K<Source, A> ma) 
        where M : MonadIO<M> =>
        M.LiftIOMaybe(ma.Iter());

    /// <summary>
    /// Force iteration of the stream, yielding the last structure processed
    /// </summary>
    public static IO<A> Last<A>(this K<Source, A> ma) =>
        ma.As()
          .ReduceIO(Option<A>.None, (_, x) => Reduced.ContinueIO(Some(x)))
          .Bind(ma => ma switch
                      {
                          { IsSome: true, Case: A value } => IO.pure(value),
                          _                               => IO.empty<A>()
                      });

    /// <summary>
    /// Force iteration of the stream, yielding the last structure processed
    /// </summary>
    public static K<M, A> Last<M, A>(this K<Source, A> ma)
        where M : MonadIO<M> =>
        M.LiftIOMaybe(ma.Last());

    /// <summary>
    /// Force iteration of the stream and collect all the values into a `Seq`.
    /// </summary>
    public static IO<Seq<A>> Collect<A>(this K<Source, A> ma) =>
        ma.As().ReduceIO<Seq<A>>([], (xs, x) => Reduced.ContinueIO(xs.Add(x)));
    
    /// <summary>
    /// Force iteration of the stream and collect all the values into a `Seq`.
    /// </summary>
    public static K<M, Seq<A>> Collect<M, A>(this K<Source, A> ma)
        where M : MonadIO<M> =>
        M.LiftIOMaybe(ma.Collect());    
}
