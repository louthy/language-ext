using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identity trait implementation
/// </summary>
public class Identity : 
    Monad<Identity>, 
    Traversable<Identity>,
    Choice<Identity>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Module
    //
    
    public static K<Identity, A> Pure<A>(A value) =>
        Identity<A>.Pure(value);
    
    public static Identity<B> bind<A, B>(Identity<A> ma, Func<A, Identity<B>> f) =>
        ma.As().Bind(f);

    public static Identity<B> map<A, B>(Func<A, B> f, Identity<A> ma) => 
        ma.As().Map(f);

    public static Identity<B> apply<A, B>(Identity<Func<A, B>> mf, Identity<A> ma) =>
        mf.Bind(ma.Map);

    public static Identity<B> action<A, B>(Identity<A> ma, Identity<B> mb) =>
        ma.Bind(_ => mb);
 
    public static S fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Identity, A> ta) => 
        f(ta.As().Value)(initialState);

    public static S foldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Identity, A> ta) => 
        f(initialState)(ta.As().Value);

    public static K<F, K<Identity, B>> traverse<F, A, B>(Func<A, K<F, B>> f, K<Identity, A> ta)
        where F : Applicative<F> =>
        F.Map(Pure, f(ta.As().Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //
    
    static K<Identity, B> Monad<Identity>.Bind<A, B>(K<Identity, A> ma, Func<A, K<Identity, B>> f) =>
        ma.As().Bind(f);

    static K<Identity, B> Functor<Identity>.Map<A, B>(Func<A, B> f, K<Identity, A> ma) => 
        ma.As().Map(f);

    static K<Identity, B> Applicative<Identity>.Apply<A, B>(K<Identity, Func<A, B>> mf, K<Identity, A> ma) =>
        mf.As().Bind(f => ma.As().Map(f));

    static K<Identity, B> Applicative<Identity>.Action<A, B>(K<Identity, A> ma, K<Identity, B> mb) =>
        ma.As().Bind(_ => mb);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Foldable
    //

    static S Foldable<Identity>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
        K<Identity, A> ta)
    {
        var id = ta.As();
        if (!predicate((initialState, id.Value))) return initialState;
        return f(id.Value)(initialState);
    }
    
    static S Foldable<Identity>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<Identity, A> ta)
    {
        var id = ta.As();
        if (!predicate((initialState, id.Value))) return initialState;
        return f(initialState)(id.Value);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Traversable
    //
    
    static K<F, K<Identity, B>> Traversable<Identity>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Identity, A> ta) =>
        F.Map(Pure, f(ta.As().Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Alternative
    //

    static K<Identity, A> Choice<Identity>.Choose<A>(K<Identity, A> fa, K<Identity, A> fb) => 
        fa;
}
