using System;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract record Transducer<A, B> : K<Transducer<A>, B>
{
    public abstract ReducerAsync<A, S> Reduce<S>(ReducerAsync<B, S> reducer);
    public abstract ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, B, S> reducer)
        where M : Applicative<M>, Alternative<M>;

    public virtual Transducer<A, C> Compose<C>(
        Transducer<B, C> tg) =>
        new ComposeTransducer<A, B, C>(this, tg);

    public Transducer<A, C> Map<C>(Func<B, C> f) =>
        Compose(Transducer.map(f));

    public Transducer<A, C> Bind<C>(Func<B, K<Transducer<A>, C>> f) =>
        new BindTransducer1<A, B, C>(this, f);

    public Transducer<A, C> Bind<C>(Func<B, Transducer<A, C>> f) =>
        new BindTransducer2<A, B, C>(this, f);

    public Transducer<A, D> SelectMany<C, D>(Func<B, K<Transducer<A>, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducer1<A, B, C, D>(this, bind, project);

    public Transducer<A, D> SelectMany<C, D>(Func<B, Transducer<A, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducer2<A, B, C, D>(this, bind, project);
    
    public Transducer<A, B> Filter(Func<B, bool> f) =>
        Compose(Transducer.filter(f));
    
    public Transducer<A, B> Where(Func<B, bool> f) =>
        Compose(Transducer.filter(f));
    
    public Transducer<A, B> Take(int n) =>
        Compose(Transducer.take<B>(n));
    
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
