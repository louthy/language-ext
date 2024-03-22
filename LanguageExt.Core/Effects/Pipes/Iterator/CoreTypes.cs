using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public abstract record Iterator<UOut, UIn, DIn, DOut, M, A>(
    Func<Proxy<UOut, UIn, DIn, DOut, M, A>> Next) 
    : Proxy<UOut, UIn, DIn, DOut, M, A>
    where M : Monad<M>
{
    public abstract IEnumerable<Proxy<UOut, UIn, DIn, DOut, M, Unit>> Run();
}

public record IteratorFoldable<UOut, UIn, DIn, DOut, F, X, M, A>(
    K<F, X> Items,
    Func<X, Proxy<UOut, UIn, DIn, DOut, M, Unit>> Yield,
    Func<Proxy<UOut, UIn, DIn, DOut, M, A>> Next) 
    : Iterator<UOut, UIn, DIn, DOut, M, A>(Next)
    where M : Monad<M>
    where F : Foldable<F>
{
    public override Proxy<UOut, UIn, DIn, DOut, M, A> ToProxy() =>
        this;

    public override IEnumerable<Proxy<UOut, UIn, DIn, DOut, M, Unit>> Run()
    {
        foreach (var item in Items.ToEnumerable())
        {
            yield return Yield(item);
        }
    }
    
    public override Proxy<UOut, UIn, DIn, DOut, M, B> Bind<B>(
        Func<A, Proxy<UOut, UIn, DIn, DOut, M, B>> f) =>
        new IteratorFoldable<UOut, UIn, DIn, DOut, F, X, M, B>(
            Items,
            Yield,
            () => Next().Bind(f));
    
    public override Proxy<UOut, UIn, DIn, DOut, M, B> Map<B>(
        Func<A, B> f) =>
        new IteratorFoldable<UOut, UIn, DIn, DOut, F, X, M, B>(
            Items,
            Yield,
            () => Next().Map(f));
    
    public override Proxy<UOut, UIn, DIn, DOut, M, B> MapM<B>(
        Func<K<M, A>, K<M, B>> f) =>
        new IteratorFoldable<UOut, UIn, DIn, DOut, F, X, M, B>(
            Items,
            Yield,
            () => Next().MapM(f));
    
    public override Proxy<UOut, UIn, C1, C, M, A> For<C1, C>(
        Func<DOut, Proxy<UOut, UIn, C1, C, M, DIn>> body) =>
        ReplaceRespond(body);
    
    public override Proxy<UOut, UIn, DIn, DOut, M, B> Action<B>(
        Proxy<UOut, UIn, DIn, DOut, M, B> r) =>
        new IteratorFoldable<UOut, UIn, DIn, DOut, F, X, M, B>(
            Items,
            Yield,
            () => Next().Action(r));    
    
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, UOut, UIn, M, A>> lhs) =>
        new IteratorFoldable<UOutA, AUInA, DIn, DOut, F, X, M, A>(
            Items,
            c1 => Yield(c1).PairEachRequestWithRespond(x => lhs(x).Map(_ => Prelude.unit)),
            () => Next().PairEachRequestWithRespond(lhs));
    
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, DIn, DOut, M, UIn>> lhs) =>
        new IteratorFoldable<UOutA, AUInA, DIn, DOut, F, X, M, A>(
            Items,
            c1 => Yield(c1).ReplaceRequest(lhs),                
            () => Next().ReplaceRequest(lhs));
    
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<DOut, Proxy<DIn, DOut, DInC, DOutC, M, A>> rhs) =>
        new IteratorFoldable<UOut, UIn, DInC, DOutC, F, X, M, A>(
            Items,
            c1 => Yield(c1).PairEachRespondWithRequest(x => rhs(x).Map(_ => Prelude.unit)),
            () => Next().PairEachRespondWithRequest(rhs));
    
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<DOut, Proxy<UOut, UIn, DInC, DOutC, M, DIn>> rhs) =>
        new IteratorFoldable<UOut, UIn, DInC, DOutC, F, X, M, A>(
            Items,
            c1 => Yield(c1).ReplaceRespond(rhs),
            () => Next().ReplaceRespond(rhs));
    
    public override Proxy<DOut, DIn, UIn, UOut, M, A> Reflect() =>
        new IteratorFoldable<DOut, DIn, UIn, UOut, F, X, M, A>(
            Items, 
            x => Yield(x).Reflect(),
            () => Next().Reflect());
    
    public override Proxy<UOut, UIn, DIn, DOut, M, A> Observe() =>
        new ProxyM<UOut, UIn, DIn, DOut, M, A>(M.Pure<Proxy<UOut, UIn, DIn, DOut, M, A>>(this));
}
