using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    /// <param name="ma"></param>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="A"></typeparam>
    extension<M, A>(K<SourceT<M>, A> ma) where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Force iteration of the stream, yielding a unit `M` structure.
        /// </summary>
        /// <remarks>
        /// The expectation is that the stream uses `IO` for side effects, so this makes them happen.
        /// </remarks>
        [Pure]
        public K<M, Unit> Iter() =>
            ma.As().FoldReduce(unit, (_, _) => unit);

        /// <summary>
        /// Force iteration of the stream, yielding the last structure processed
        /// </summary>
        [Pure]
        public K<M, A> Last() =>
            M.Token.Bind(t =>
                             ma.As()
                               .FoldReduce(Option<A>.None, (_, x) => Some(x))
                               .Bind(ma => ma switch
                                           {
                                               { IsSome: true, Case: A value } => M.Pure(value),
                                               _                               => M.Empty<A>()
                                           }));

        /// <summary>
        /// Collect all the values into a `Seq` while the predicate holds.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public K<M, Seq<A>> CollectWhile(Func<(Seq<A> Items, A Item), bool> predicate) =>
            ma.As().Reduce<Seq<A>>(
                [], 
                (xs, x) => predicate((xs, x))
                               ? Reduced.Continue(xs.Add(x))
                               : Reduced.Done(xs));

        /// <summary>
        /// Collect all the values into a `Seq` while the predicate holds.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public K<M, Seq<A>> CollectUntil(Func<(Seq<A> Items, A Item), bool> predicate) =>
            ma.As().Reduce<Seq<A>>(
                [], 
                (xs, x) => predicate((xs, x))
                               ? Reduced.Done(xs)
                               : Reduced.Continue(xs.Add(x)));

        /// <summary>
        /// Force iteration of the stream and collect all the values into a `Seq`.
        /// </summary>
        [Pure]
        public K<M, Seq<A>> Collect() =>
            ma.As().FoldReduce<Seq<A>>([], (xs, x) => xs.Add(x));
    }
}
