using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Validation :
    Coproduct<Validation>,
    Bimonad<Validation>
{
    static K<Validation, A, B> CoproductCons<Validation>.Left<A, B>(A value) => 
        FailI<A, B>(value);

    static K<Validation, A, B> CoproductCons<Validation>.Right<A, B>(B value) => 
        SuccessI<A, B>(value);

    static C Coproduct<Validation>.Match<A, B, C>(
        Func<A, C> Left, 
        Func<B, C> Right, 
        K<Validation, A, B> fab) => 
        fab.As2().Match(Fail: Left, Succ: Right);

    static K<Validation, M, B> Bifunctor<Validation>.BiMap<L, A, M, B>(
        Func<L, M> first, 
        Func<A, B> second, 
        K<Validation, L, A> fab) => 
        fab.As2().BiMap(Succ: second, Fail: first);

    static K<Validation, Y, A> Bimonad<Validation>.BindFirst<X, Y, A>(
        K<Validation, X, A> ma, 
        Func<X, K<Validation, Y, A>> f) => 
        ma.As2().BiBind(x => f(x).As2(), SuccessI<Y, A>);

    static K<Validation, X, B> Bimonad<Validation>.BindSecond<X, A, B>(K<Validation, X, A> ma, Func<A, K<Validation, X, B>> f) => 
        ma.As2().BiBind(FailI<X, B>, x => f(x).As2());
}
