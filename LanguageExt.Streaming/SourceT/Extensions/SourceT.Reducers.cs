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
    extension<M, A>(K<SourceT<M>, A> ma) where M : MonadIO<M>
    {
        /// <summary>
        /// Reduce the stream into a unit value.
        /// </summary>
        [Pure]
        public K<M, Unit> Iter() =>
            ma.As().FoldReduce(unit, (_, _) => unit);

        /// <summary>
        /// Reduce the stream by collecting all the values into a `Seq` while the predicate holds.
        /// </summary>
        [Pure]
        public K<M, Seq<A>> CollectWhile(Func<(Seq<A> Items, A Item), bool> predicate) =>
            ma.As().Reduce<Seq<A>>(
                [], 
                (xs, x) => predicate((xs, x))
                               ? Reduced.Continue(xs.Add(x))
                               : Reduced.Done(xs));

        /// <summary>
        /// Reduce the stream by collecting all the values into a `Seq` while the predicate holds.
        /// </summary>
        [Pure]
        public K<M, Seq<A>> CollectUntil(Func<(Seq<A> Items, A Item), bool> predicate) =>
            ma.As().Reduce<Seq<A>>(
                [], 
                (xs, x) => predicate((xs, x))
                               ? Reduced.Done(xs)
                               : Reduced.Continue(xs.Add(x)));

        /// <summary>
        /// Reduce the stream by collecting all the values into a `Seq`.
        /// </summary>
        [Pure]
        public K<M, Seq<A>> Collect() =>
            ma.As().FoldReduce<Seq<A>>([], (xs, x) => xs.Add(x));
        
        /// <summary>
        /// Reduce the stream, yielding the last structure processed, or the `None` if the stream is empty
        /// </summary>
        [Pure]
        public K<M, Option<A>> LastOrNone() =>
            ma.As().FoldReduce(Option<A>.None, (_, x) => Some(x));
        
        /// <summary>
        /// Reduce the stream, yielding the first structure processed, or the `None` if the stream is empty
        /// </summary>
        [Pure]
        public K<M, Option<A>> FirstOrNone() =>
            ma.As().Reduce(Option<A>.None, (_, x) => Reduced.Done(Some(x)));
        
        /// <summary>
        /// Reduce the stream, yielding the last structure processed, or the default value if the stream is empty
        /// </summary>
        [Pure]
        public K<M, A> Last(A defaultValue) =>
            ma.As().FoldReduce(defaultValue, (_, x) => x);

        /// <summary>
        /// Reduce the stream, yielding the first structure processed, or the default value if the stream is empty
        /// </summary>
        [Pure]
        public K<M, A> First(A defaultValue) =>
            ma.As().Reduce(defaultValue, (_, x) => Reduced.Done(x));
    }    
    
    /// <param name="ma"></param>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="A"></typeparam>
    extension<M, A>(K<SourceT<M>, A> ma) 
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Reduce the stream, yielding the last structure processed, or `M.Empty` if the stream is empty
        /// </summary>
        [Pure]
        public K<M, A> Last() =>
            ma.LastOrNone()
              .Bind(ma => ma switch
                          {
                              { IsSome: true, Case: A value } => M.Pure(value),
                              _                               => M.Empty<A>()
                          });
        
        /// <summary>
        /// Reduce the stream, yielding the first structure processed, or `M.Empty` if the stream is empty
        /// </summary>
        [Pure]
        public K<M, A> First() =>
            ma.FirstOrNone()
              .Bind(ma => ma switch
                          {
                              { IsSome: true, Case: A value } => M.Pure(value),
                              _                               => M.Empty<A>()
                          });        
    }
}
