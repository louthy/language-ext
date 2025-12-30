using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    /// <param name="ma">Stream of values to delay the yielding of</param>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    extension<M, A>(K<SourceT<M>, A> ma) where M : MonadIO<M>
    {
        /// <summary>
        /// Delay the yielding of values by the specified duration
        /// </summary>
        /// <param name="duration">Duration to delay the yielding of values for</param>
        /// <returns></returns>
        public SourceT<M, A> Delay(TimeSpan duration) =>
            IO.yieldFor(duration) >> ma >> lower;

        /// <summary>
        /// Delay the yielding of values by the specified duration
        /// </summary>
        /// <param name="duration">Duration to delay the yielding of values for</param>
        /// <returns></returns>
        public SourceT<M, A> Delay(Duration duration) =>
            IO.yieldFor(duration) >> ma >> lower;
    }

    /// <param name="ma">Source</param>
    /// <typeparam name="M">Lifted monad trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    extension<M, A>(K<SourceT<M>, A> ma) where M : MonadIO<M>, Fallible<M>
    {
        /// <summary>
        /// Take values from the source for a period of time 
        /// </summary>
        /// <param name="duration">Duration to take values for</param>
        /// <returns>SourceT</returns>
        [Pure]
        public SourceT<M, A> TakeFor(Duration duration) =>
            new TakeForSourceT<M, A>(+ma, (TimeSpan)duration);

        /// <summary>
        /// Take values from the source for a period of time 
        /// </summary>
        /// <param name="duration">Duration to take values for</param>
        /// <returns>SourceT</returns>
        [Pure]
        public SourceT<M, A> TakeFor(TimeSpan duration) =>
            new TakeForSourceT<M, A>(+ma, duration);
        

        /// <summary>
        /// Combine two sources into a single source.  The value streams are both
        /// merged into a new stream.  Values are yielded as they become available
        /// regardless of which stream yields it.
        /// </summary>
        /// <param name="this">Left-hand side</param>
        /// <param name="rhs">Right-hand side</param>
        /// <returns>Merged stream of values</returns>
        public SourceT<M, A> Choose(K<SourceT<M>, A> rhs) =>
            (ma, rhs) switch
            {
                (EmptySourceT<M, A>, EmptySourceT<M, A>)       => EmptySourceT<M, A>.Default,
                (var l, EmptySourceT<M, A>)                    => +l,
                (EmptySourceT<M, A>, var r)                    => +r,
                (ChooseSourceT<M, A> l, ChooseSourceT<M, A> r) => new ChooseSourceT<M, A>(l.Sources + r.Sources),
                (ChooseSourceT<M, A> l, var r)                 => new ChooseSourceT<M, A>(l.Sources.Add(+r)),
                (var l, ChooseSourceT<M, A> r)                 => new ChooseSourceT<M, A>(l.As().Cons(r.Sources)),
                var (l, r)                                     => new ChooseSourceT<M, A>([+l, +r])
            };

        /// <summary>
        /// Combine two sources into a single source.  The value streams are both
        /// merged into a new stream.  Values are yielded as they become available
        /// regardless of which stream yields it.
        /// </summary>
        /// <param name="this">Left-hand side</param>
        /// <param name="rhs">Right-hand side</param>
        /// <returns>Merged stream of values</returns>
        public SourceT<M, A> Choose(Memo<SourceT<M>, A> rhs) =>
            ma.Choose(rhs.Value.As());        
    }

    /// <param name="first">Stream to zip</param>
    extension<M, A>(K<SourceT<M>, A> first) where M : MonadIO<M>, Fallible<Error, M>
    {
        /// <summary>
        /// Zip two sources into one
        /// </summary>
        /// <param name="second">Stream to zip</param>
        /// <returns>Stream of values where the items from two streams are paired together</returns>
        public SourceT<M, (A First, B Second)> Zip<B>(K<SourceT<M>, B> second) =>
            new Zip2SourceT<M, A, B>(first.As(), second.As());

        /// <summary>
        /// Zip three sources into one
        /// </summary>
        /// <param name="second">Stream to zip</param>
        /// <param name="third">Stream to zip</param>
        /// <returns>Stream of values where the items from two streams are paired together</returns>
        public SourceT<M, (A First, B Second, C Third)> Zip<B, C>(
            K<SourceT<M>, B> second, 
            K<SourceT<M>, C> third) =>
            new Zip3SourceT<M, A, B, C>(first.As(), second.As(), third.As());

        /// <summary>
        /// Zip three sources into one
        /// </summary>
        /// <param name="second">Stream to zip</param>
        /// <param name="third">Stream to zip</param>
        /// <param name="fourth">Stream to zip</param>
        /// <returns>Stream of values where the items from two streams are paired together</returns>
        public SourceT<M, (A First, B Second, C Third, D Fourth)> Zip<B, C, D>(
            K<SourceT<M>, B> second,
            K<SourceT<M>, C> third,
            K<SourceT<M>, D> fourth) =>
            new Zip4SourceT<M, A, B, C, D>(first.As(), second.As(), third.As(), fourth.As());
    }
}
