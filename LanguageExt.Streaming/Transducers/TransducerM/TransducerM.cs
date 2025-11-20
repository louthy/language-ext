using System;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract record TransducerM<M, A, B> : 
    K<TransduceFromM<M, A>, B>,
    K<TransduceToM<M, B>, A>
{
    /// <summary>
    /// Fold the input stream using the supplied reducer.  
    /// </summary>
    /// <param name="reducer">Reducer that folds the stream of values flowing through the transducer</param>
    /// <typeparam name="S">State</typeparam>
    /// <returns></returns>
    public abstract ReducerM<M, A, S> Reduce<S>(ReducerM<M, B, S> reducer);

    /// <summary>
    /// Compose two transducers together.  The output of the first transducer is the input to the second.
    /// </summary>
    public virtual TransducerM<M, A, C> Compose<C>(TransducerM<M, B, C> tg) =>
        new ComposeTransducerM<M, A, B, C>(this, tg);

    /// <summary>
    /// Functor map 
    /// </summary>
    public TransducerM<M, A, C> Map<C>(Func<B, C> f) =>
        Compose(TransducerM.map<M, B, C>(f));

    /// <summary>
    /// Contravariant functor map 
    /// </summary>
    public TransducerM<M, X, B> Comap<X>(Func<X, A> f) =>
        TransducerM.map<M, X, A>(f).Compose(this);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public TransducerM<M, A, C> Bind<C>(Func<B, K<TransduceFromM<M, A>, C>> f) =>
        new BindTransducerM1<M, A, B, C>(this, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public TransducerM<M, A, C> Bind<C>(Func<B, TransducerM<M, A, C>> f) =>
        new BindTransducerM2<M, A, B, C>(this, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public TransducerM<M, A, D> SelectMany<C, D>(Func<B, K<TransduceFromM<M, A>, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducerM1<M, A, B, C, D>(this, bind, project);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public TransducerM<M, A, D> SelectMany<C, D>(Func<B, TransducerM<M, A, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducerM2<M, A, B, C, D>(this, bind, project);
}
