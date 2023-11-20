using System;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt;

public abstract record Fold<A, S> : Transducer<A, S>
{
    public abstract Fold<A, B> Map<B>(Func<S, B> f);
    public abstract Fold<A, B> Bind<B>(Func<S, Fold<A, B>> f);
    public abstract Transducer<A, S> ToTransducer();

    public Fold<A, C> SelectMany<B, C>(Func<S, Fold<A, B>> bind, Func<S, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public static Fold<A, S> New(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new FoldIdentity<A, S>(
            schedule,
            initialState,
            folder,
            predicate);

    public Transducer<A, S> Morphism =>
        ToTransducer();
    
    public Reducer<A, S1> Transform<S1>(Reducer<S, S1> reduce) => 
        Morphism.Transform(reduce);
}

record FoldIdentity<A, S>(
    Schedule Schedule,
    S InitialState,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate) : Fold<A, S>
{
    public override Fold<A, B> Map<B>(Func<S, B> f) => 
        new FoldMap<S, A, B>(this, f);

    public override Fold<A, B> Bind<B>(Func<S, Fold<A, B>> f) => 
        new FoldBind<S, A, B>(this, f);

    public override Transducer<A, S> ToTransducer() =>
        Transducer.fold(
            Schedule,
            InitialState,
            Folder,
            s => Predicate(s) ? TResult.Continue<Unit>(default) : TResult.Complete<Unit>(default));
}

record FoldMap<S, A, B>(Fold<A, S> Target, Func<S, B> F) : Fold<A, B>
{
    public override Fold<A, C> Map<C>(Func<B, C> f) =>
        new FoldMap<S, A, C>(Target, x => f(F(x)));

    public override Fold<A, C> Bind<C>(Func<B, Fold<A, C>> f) =>
        new FoldBind<S, A, C>(Target, x => f(F(x)));
    
    public override Transducer<A, B> ToTransducer() =>
        Transducer.compose(Target.ToTransducer(), Transducer.lift(F));
}

record FoldBind<S, A, B>(Fold<A, S> Target, Func<S, Fold<A, B>> F) : Fold<A, B>
{
    public override Fold<A, C> Map<C>(Func<B, C> f) => 
        new FoldBind<S, A, C>(Target, x => F(x).Map(f));

    public override Fold<A, C> Bind<C>(Func<B, Fold<A, C>> f) => 
        new FoldBind<S, A, C>(Target, x => F(x).Bind(f));

    public override Transducer<A, B> ToTransducer() =>
        Transducer.bind(Target.ToTransducer(), s => F(s).ToTransducer());
}
