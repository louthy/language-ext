using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

/// <summary>
/// A source / stream of values
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record Source<A> : 
    K<Source, A>, 
    Monoid<Source<A>>
{
    /// <summary>
    /// A source that never yields a value
    /// </summary>
    public static Source<A> Empty =>
        EmptySource<A>.Default;

    /// <summary>
    /// Get an iterator of source values
    /// </summary>
    /// <returns>Source iterator</returns>
    public abstract SourceIterator<A> GetIterator();

    /// <summary>
    /// Reduce the stream to a single value
    /// </summary>
    /// <param name="ma"></param>
    /// <param name="initial"></param>
    /// <param name="reducer"></param>
    /// <typeparam name="S"></typeparam>
    /// <returns></returns>
    public abstract S Reduce<S>(K<Source, A> ma, S initial, Reducer<S, A> reducer);
    
    /// <summary>
    /// Functor map
    /// </summary>
    public virtual Source<B> Map<B>(Func<A, B> f) =>
        new TransformSource<A, B>(this, Transducer.map(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public virtual Source<B> Bind<B>(Func<A, Source<B>> f) =>
        new BindSource<A, B>(this, f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<B> Bind<B>(Func<A, K<Source, B>> f) =>
        Bind(x => f(x).As());
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>Source where the only values yield are those that pass the predicate</returns>
    public Source<A> Where(Func<A, bool> f) =>
        new TransformSource<A, A>(this, Transducer.filter(f));
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>Source where the only values yield are those that pass the predicate</returns>
    public Source<A> Filter(Func<A, bool> f) =>
        new TransformSource<A, A>(this, Transducer.filter(f));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public virtual Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        new ApplySource<A, B>(this, ff);

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public Source<A> Combine(Source<A> rhs) =>
        (this, rhs) switch
        {
            (EmptySource<A>, EmptySource<A>)         => EmptySource<A>.Default,
            (var l, EmptySource<A>)                  => l,
            (EmptySource<A>, var r)                  => r,
            (CombineSource<A> l, CombineSource<A> r) => new CombineSource<A>(l.Sources + r.Sources),
            (CombineSource<A> l, var r)              => new CombineSource<A>(l.Sources.Add(r)),
            (var l, CombineSource<A> r)              => new CombineSource<A>(l.Cons(r.Sources)),
            _                                        => new CombineSource<A>([this, rhs])
        };
    
    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceClosed` is raised</returns>
    public Source<A> Choose(Source<A> rhs) =>
        new SourceChoose<A>(this, rhs);

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second)> Zip<B>(Source<B> second) =>
        new Zip2Source<A, B>(this, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third)> Zip<B, C>(Source<B> second, Source<C> third) =>
        new Zip3Source<A, B, C>(this, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third, D Fourth)> Zip<B, C, D>(Source<B> second, Source<C> third, Source<D> fourth) =>
        new Zip4Source<A, B, C, D>(this, second, third, fourth);

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public Source<S> Fold<S>(Func<S, A, S> Fold, S Init) =>
        new FoldWhileSource<S, A>(Schedule.Forever, Fold, _ => true, Init, this);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, or the
    /// source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> Fold<S>(Schedule Time, Func<S, A, S> Fold, S Init) =>
        new FoldWhileSource<S, A>(Time, Fold, _ => true, Init, this);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldWhile<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        new FoldWhileSource<S, A>(Schedule.Forever, Fold, Pred, Init, this);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldUntil<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        new FoldUntilSource<S, A>(Schedule.Forever, Fold, Pred, Init, this);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, the
    /// predicate returns `false`, or the source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldWhile<S>(
        Schedule Time,
        Func<S, A, S> Fold, 
        Func<(S State, A Value), bool> Pred, 
        S Init) =>
        new FoldWhileSource<S, A>(Time, Fold, Pred, Init, this);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, the
    /// predicate returns `true`, or the source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S"></typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldUntil<S>(
        Schedule Time,
        Func<S, A, S> Fold, 
        Func<(S State, A Value), bool> Pred, 
        S Init) =>
        new FoldUntilSource<S, A>(Time, Fold, Pred, Init, this);

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT<M>()
        where M : Monad<M> =>
        PipeT.lift<Unit, A, M, SourceIterator<A>>(GetIterator)
             .Bind(iter => PipeT.yieldRepeatIO<M, Unit, A>(iter.Read()));

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public Producer<RT, A, Unit> ToProducer<RT>() =>
        ToProducerT<Eff<RT>>();
    
    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
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
    /// `rhs` is also empty then `Errors.SourceClosed` is raised</returns>
    public static Source<A> operator |(Source<A> lhs, Source<A> rhs) =>
        lhs.Choose(rhs);

    /// <summary>
    /// Functor map
    /// </summary>
    public Source<B> Select<B>(Func<A, B> f) =>
        Map(f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<C> SelectMany<B, C>(Func<A, Source<B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));
}
