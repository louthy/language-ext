using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identity trait implementation
/// </summary>
public partial class Identity : 
    Monad<Identity>, 
    Traversable<Identity>,
    Choice<Identity>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //
    
    static K<Identity, B> Monad<Identity>.Bind<A, B>(K<Identity, A> ma, Func<A, K<Identity, B>> f) =>
        ma.As().Bind(f);

    static K<Identity, B> Functor<Identity>.Map<A, B>(Func<A, B> f, K<Identity, A> ma) => 
        ma.As().Map(f);

    static K<Identity, A> Applicative<Identity>.Pure<A>(A value) =>
        new Identity<A>(value);

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
        F.Map(PureK, f(ta.As().Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Alternative
    //

    static K<Identity, A> Choice<Identity>.Choose<A>(K<Identity, A> lhs, K<Identity, A> rhs) => 
        lhs;

    static K<Identity, A> Choice<Identity>.Choose<A>(K<Identity, A> lhs, Func<K<Identity, A>> rhs) => 
        lhs;

    static K<Identity, A> SemigroupK<Identity>.Combine<A>(K<Identity, A> lhs, K<Identity, A> rhs) =>
        lhs.Choose(rhs);
}
