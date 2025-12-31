using System;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract record Transducer<A, B> : 
    K<TransduceFrom<A>, B>,
    K<TransduceTo<B>, A>
{
    /// <summary>
    /// Fold the input stream using the supplied reducer.  
    /// </summary>
    /// <param name="reducer">Reducer that folds the stream of values flowing through the transducer</param>
    /// <typeparam name="S">State</typeparam>
    /// <returns></returns>
    public abstract ReducerIO<A, S> Reduce<S>(ReducerIO<B, S> reducer);

    /// <summary>
    /// Lift the pure value transducer into a `M` value transducer
    /// </summary>
    /// <typeparam name="M">Lifted space</typeparam>
    /// <returns>Lifted transducer</returns>
    public abstract TransducerM<M, A, B> Lift<M>()
        where M : Applicative<M>;
    
    /// <summary>
    /// Compose two transducers together.  The output of the first transducer is the input to the second.
    /// </summary>
    public virtual Transducer<A, C> Compose<C>(Transducer<B, C> tg) =>
        new ComposeTransducer<A, B, C>(this, tg);

    /// <summary>
    /// Functor map 
    /// </summary>
    public Transducer<A, C> Map<C>(Func<B, C> f) =>
        Compose(Transducer.map(f));

    /// <summary>
    /// Contravariant functor map 
    /// </summary>
    public Transducer<X, B> Comap<X>(Func<X, A> f) =>
        Transducer.map(f).Compose(this);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public Transducer<A, C> Bind<C>(Func<B, K<TransduceFrom<A>, C>> f) =>
        new BindTransducer1<A, B, C>(this, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public Transducer<A, C> Bind<C>(Func<B, Transducer<A, C>> f) =>
        new BindTransducer2<A, B, C>(this, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public Transducer<A, D> SelectMany<C, D>(Func<B, K<TransduceFrom<A>, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducer1<A, B, C, D>(this, bind, project);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public Transducer<A, D> SelectMany<C, D>(Func<B, Transducer<A, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducer2<A, B, C, D>(this, bind, project);

    /// <summary>
    /// Filter values flowing through the transducer. 
    /// </summary>
    public Transducer<A, B> Filter(Func<B, bool> f) =>
        Compose(Transducer.filter(f));
    
    /// <summary>
    /// Filter values flowing through the transducer. 
    /// </summary>
    public Transducer<A, B> Where(Func<B, bool> f) =>
        Compose(Transducer.filter(f));

    /// <summary>
    /// Take the first `n` values from the stream.  
    /// </summary>
    public Transducer<A, B> Take(int n) =>
        Compose(Transducer.take<B>(n));

    /// <summary>
    /// Skip the first `n` values from the stream. 
    /// </summary>
    public Transducer<A, B> Skip(int n) =>
        Compose(Transducer.skip<B>(n));

    public Transducer<A, S> FoldWhile<S>(
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) =>
        Compose(Transducer.foldWhile(Folder, Pred, State));
    
    public Transducer<A, S> FoldWhile<S>(
        Schedule Schedule, 
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) =>
        Compose(Transducer.foldWhile(Schedule, Folder, Pred, State));
    
    public Transducer<A, S> FoldUntil<S>(
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) =>
        Compose(Transducer.foldUntil(Folder, Pred, State));
    
    public Transducer<A, S> FoldUntil<S>(
        Schedule Schedule, 
        Func<S, B, S> Folder, 
        Func<S, B, bool> Pred, 
        S State) =>
        Compose(Transducer.foldUntil(Schedule, Folder, Pred, State));    
}
