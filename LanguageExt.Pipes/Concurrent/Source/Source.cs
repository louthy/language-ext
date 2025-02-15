using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

public abstract record Source<A> : 
    K<Source, A>, 
    Monoid<Source<A>>
{
    /// <summary>
    /// An Source that never yields a value
    /// </summary>
    public static Source<A> Empty =>
        SourceEmpty<A>.Default;

    /// <summary>
    /// Read value from the Source
    /// </summary>
    /// <remarks>
    /// Raises a `Errors.SourceChannelClosed` if the channel is closed or empty
    /// </remarks>
    /// <returns>First available value from the channel</returns>
    public abstract IO<A> Read();
    
    /// <summary>
    /// Functor map
    /// </summary>
    public abstract Source<B> Map<B>(Func<A, B> f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public abstract Source<B> Bind<B>(Func<A, Source<B>> f);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public abstract Source<B> ApplyBack<B>(Source<Func<A, B>> ff);

    /// <summary>
    /// Combine two Sourcees into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public Source<A> Combine(Source<A> rhs) =>
        (this, rhs) switch
        {
            (SourceEmpty<A>, SourceEmpty<A>)         => SourceEmpty<A>.Default,
            (var l, SourceEmpty<A>)                  => l,
            (SourceEmpty<A>, var r)                  => r,
            (SourceCombine<A> l, SourceCombine<A> r) => new SourceCombine<A>(l.Sources + r.Sources),
            (SourceCombine<A> l, var r)              => new SourceCombine<A>(l.Sources.Add(r)),
            (var l, SourceCombine<A> r)              => new SourceCombine<A>(l.Cons(r.Sources)),
            _                                        => new SourceCombine<A>([this, rhs])
        };
    
    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceChannelClosed` is raised</returns>
    public Source<A> Choose(Source<A> rhs) =>
        new SourceChoose<A>(this, rhs);

    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<B> Bind<B>(Func<A, K<Source, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT<M>()
        where M : Monad<M> =>
        PipeT.yieldRepeatIO<M, Unit, A>(Read());

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public Producer<RT, A, Unit> ToProducer<RT>() =>
        ToProducerT<Eff<RT>>();
    
    /// <summary>
    /// Combine two Sourcees into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Source<A> operator +(Source<A> lhs, Source<A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Value from the `lhs` `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceChannelClosed` is raised</returns>
    public static Source<A> operator |(Source<A> lhs, Source<A> rhs) =>
        lhs.Choose(rhs);

    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);
}
