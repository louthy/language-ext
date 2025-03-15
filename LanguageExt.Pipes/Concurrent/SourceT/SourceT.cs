using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

/// <summary>
/// A source / stream of values
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record SourceT<M, A> : 
    K<SourceT<M>, A>, 
    Monoid<SourceT<M, A>>
    where M : Monad<M>, Alternative<M>
{
    public abstract K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer);
   // TODO: public abstract K<M, S> Reduce<S>(K<M, S> state, ReducerM<M, A, S> reducer);
    
    /// <summary>
    /// A source that never yields a value
    /// </summary>
    public static SourceT<M, A> Empty =>
        EmptySourceT<M, A>.Default;

    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public SourceT<M, B> Transform<B>(Transducer<A, B> transducer) =>
        new TransformSourceT<M, A, B>(this, transducer);
    
    /// <summary>
    /// Functor map
    /// </summary>
    public virtual SourceT<M, B> Map<B>(Func<A, B> f) =>
        Transform(Transducer.map(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public virtual SourceT<M, B> Bind<B>(Func<A, SourceT<M, B>> f) =>
        new BindSourceT<M, A, B>(this, f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, B> Bind<B>(Func<A, K<SourceT<M>, B>> f) =>
        Bind(x => f(x).As());
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public SourceT<M, A> Where(Func<A, bool> f) =>
        Transform(Transducer.filter(f));
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public SourceT<M, A> Filter(Func<A, bool> f) =>
        Transform(Transducer.filter(f));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public virtual SourceT<M, B> ApplyBack<B>(SourceT<M, Func<A, B>> ff) =>
        new ApplySourceT<M, A, B>(ff, this);

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public SourceT<M, A> Combine(SourceT<M, A> rhs) =>
        (this, rhs) switch
        {
            (EmptySourceT<M, A>, EmptySourceT<M, A>)         => EmptySourceT<M, A>.Default,
            (var l, EmptySourceT<M, A>)                      => l,
            (EmptySourceT<M, A>, var r)                      => r,
            (CombineSourceT<M, A> l, CombineSourceT<M, A> r) => new CombineSourceT<M, A>(l.SourceTs + r.SourceTs),
            (CombineSourceT<M, A> l, var r)                  => new CombineSourceT<M, A>(l.SourceTs.Add(r)),
            (var l, CombineSourceT<M, A> r)                  => new CombineSourceT<M, A>(l.Cons(r.SourceTs)),
            _                                                => new CombineSourceT<M, A>([this, rhs])
        };
    
    /// <summary>
    /// Choose a value from the first `SourceT` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `SourceT` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceTClosed` is raised</returns>
    public SourceT<M, A> Choose(SourceT<M, A> rhs) =>
        new ChooseSourceT<M, A>(this, rhs);

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public SourceT<M, (A First, B Second)> Zip<B>(SourceT<M, B> second) =>
        new Zip2SourceT<M, A, B>(this, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public SourceT<M, (A First, B Second, C Third)> Zip<B, C>(SourceT<M, B> second, SourceT<M, C> third) =>
        new Zip3SourceT<M, A, B, C>(this, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public SourceT<M, (A First, B Second, C Third, D Fourth)> Zip<B, C, D>(
        SourceT<M, B> second, 
        SourceT<M, C> third, 
        SourceT<M, D> fourth) =>
        new Zip4SourceT<M, A, B, C, D>(this, second, third, fourth);

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public SourceT<M, A> Skip(int amount) =>
        Transform(Transducer.skip<A>(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Amount to take</param>
    /// <returns>Transformed source</returns>
    public SourceT<M, A> Take(int amount) =>
        Transform(Transducer.take<A>(amount)); 

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public SourceT<M, S> Fold<S>(Func<S, A, S> Fold, S Init) =>
        Transform(Transducer.foldWhile(Fold, _ => true, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, or the
    /// source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public SourceT<M, S> Fold<S>(Schedule Time, Func<S, A, S> Fold, S Init) =>
        Transform(Transducer.foldWhile(Time, Fold, _ => true, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public SourceT<M, S> FoldWhile<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        Transform(Transducer.foldWhile(Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public SourceT<M, S> FoldUntil<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        Transform(Transducer.foldUntil(Fold, Pred, Init));

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
    public SourceT<M, S> FoldWhile<S>(
        Schedule Time,
        Func<S, A, S> Fold, 
        Func<(S State, A Value), bool> Pred, 
        S Init) =>
        Transform(Transducer.foldWhile(Time, Fold, Pred, Init));

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
    public SourceT<M, S> FoldUntil<S>(
        Schedule Time,
        Func<S, A, S> Fold, 
        Func<(S State, A Value), bool> Pred, 
        S Init) =>
        Transform(Transducer.foldUntil(Time, Fold, Pred, Init));

    /// <summary>
    /// Convert `SourceT` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT() =>
        PipeT.lift<Unit, A, M, SourceTIterator<M, A>>(GetIterator)
             .Bind(iter => PipeT.yieldRepeat<M, Unit, A>(iter.Read()));
    
    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static SourceT<M, A> operator +(SourceT<M, A> lhs, SourceT<M, A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `SourceT` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Value from the `lhs` `SourceT` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceTClosed` is raised</returns>
    public static SourceT<M, A> operator |(SourceT<M, A> lhs, SourceT<M, A> rhs) =>
        lhs.Choose(rhs);

    /// <summary>
    /// Functor map
    /// </summary>
    public SourceT<M, B> Select<B>(Func<A, B> f) =>
        Map(f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, SourceT<M, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(a => SourceT.liftIO<M, C>(bind(a).Map(b => project(a, b))));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(a => project(a, bind(a).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Internal
    //
    
    internal abstract SourceTIterator<M, A> GetIterator();
}
