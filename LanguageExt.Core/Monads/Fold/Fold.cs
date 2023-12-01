using System;
using LanguageExt.Transducers;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class FoldExtensions
{
    public static Fold<A, S> Flatten<A, S>(this Fold<A, Fold<A, S>> mma) =>
        mma.Bind(identity);
}

public abstract record Fold<A, S> : Transducer<A, S>
{
    public abstract Fold<A, B> Map<B>(Func<S, B> f);
    public abstract Fold<A, B> Bind<B>(Func<S, Fold<A, B>> f);

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

    public abstract Transducer<A, S> Morphism { get; }

    public Reducer<A, S1> Transform<S1>(Reducer<S, S1> reduce) => 
        Morphism.Transform(reduce);

    public static Transducer<Unit, S> operator |(Pure<A> x, Fold<A, S> y) =>
        Transducer.compose(x.Morphism, y.Morphism);

    public static Transducer<Unit, S> operator |(Lift<Unit, A> x, Fold<A, S> y) =>
        Transducer.compose(x.Morphism, y.Morphism);

    public static Transducer<Unit, S> operator |(A x, Fold<A, S> y) =>
        Transducer.compose(Transducer.Pure(x), y.Morphism);

    public override string ToString() =>
        "fold";
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

    public override Transducer<A, S> Morphism =>
        Transducer.fold(
            Schedule,
            InitialState,
            Folder,
            s => Predicate(s) ? TResult.Continue<Unit>(default) : TResult.Complete<Unit>(default));
    
    public override string ToString() =>
        "fold-id";
}

record FoldConst<S, A>(A Value, Fold<A, S> Target) : Fold<Unit, S>
{
    public override Fold<Unit, B> Map<B>(Func<S, B> f) =>
        new FoldConst<B, A>(Value, Target.Map(f));

    public override Fold<Unit, B> Bind<B>(Func<S, Fold<Unit, B>> f) => 
        Map(f).Flatten();

    public override Transducer<Unit, S> Morphism =>
        Transducer.compose(Transducer.constant<Unit, A>(Value), Target.Morphism);
    
    public override string ToString() =>
        "fold-const";
}

record FoldMap<S, A, B>(Fold<A, S> Target, Func<S, B> F) : Fold<A, B>
{
    public override Fold<A, C> Map<C>(Func<B, C> f) =>
        new FoldMap<S, A, C>(Target, x => f(F(x)));

    public override Fold<A, C> Bind<C>(Func<B, Fold<A, C>> f) =>
        new FoldBind<S, A, C>(Target, x => f(F(x)));
    
    public override Transducer<A, B> Morphism =>
        Transducer.compose(Target.Morphism, Transducer.lift(F));
    
    public override string ToString() =>
        "fold-map";
}

record FoldBind<S, A, B>(Fold<A, S> Target, Func<S, Fold<A, B>> F) : Fold<A, B>
{
    public override Fold<A, C> Map<C>(Func<B, C> f) => 
        new FoldBind<S, A, C>(Target, x => F(x).Map(f));

    public override Fold<A, C> Bind<C>(Func<B, Fold<A, C>> f) => 
        new FoldBind<S, A, C>(Target, x => F(x).Bind(f));

    public override Transducer<A, B> Morphism =>
        Transducer.bind(Target.Morphism, s => F(s).Morphism);
    
    public override string ToString() =>
        "fold-bind";
}
