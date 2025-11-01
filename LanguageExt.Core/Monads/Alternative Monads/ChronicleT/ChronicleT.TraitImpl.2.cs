using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using NSE = System.NotSupportedException;

namespace LanguageExt;

/// <summary>
/// `ChronicleT` trait implementations
/// </summary>
/// <typeparam name="M">Lifted monad type</typeparam>
public class ChronicleT<M> :  
    CoproductK<ChronicleT<M>>,
    Bimonad<ChronicleT<M>> 
    where M : Monad<M>
{
    static K<ChronicleT<M>, A, B> CoproductCons<ChronicleT<M>>.Left<A, B>(A value) => 
        ChronicleT.confess<A, M, B>(value);

    static K<ChronicleT<M>, A, B> CoproductCons<ChronicleT<M>>.Right<A, B>(B value) => 
        ChronicleT.dictate<A, M, B>(value);

    static K<ChronicleT<M>, A, C> CoproductK<ChronicleT<M>>.Match<A, B, C>(
        Func<A, C> Left, 
        Func<B, C> Right, 
        K<ChronicleT<M>, A, B> fab) => 
        fab.As2().Memento().Map(ea => ea.Match(Left, Right));

    static K<ChronicleT<M>, Y, B> Bifunctor<ChronicleT<M>>.BiMap<X, A, Y, B>(
        Func<X, Y> first,
        Func<A, B> second,
        K<ChronicleT<M>, X, A> fab) =>
        new ChronicleT<Y, M, B>(
            _ => SemigroupInstance<X>.Instance switch
             {
                 { IsSome: true, Value: { } t } =>
                     fab.As2()
                        .Run(t)
                        .Map(ch => ch switch
                                   {
                                       These<X, A>.This (var x) => This<Y, B>(
                                           first(x)),
                                       These<X, A>.That (var a) => That<Y, B>(
                                           second(a)),
                                       These<X, A>.Both (var x, var a) => Both(
                                           first(x), second(a)),
                                       _ => throw new NSE()
                                   }),
                 _ => throw new NSE($"Type {typeof(X).Name} is not a valid semigroup")
             });

    static K<ChronicleT<M>, Y, A> Bimonad<ChronicleT<M>>.BindFirst<X, Y, A>(
        K<ChronicleT<M>, X, A> ma,
        Func<X, K<ChronicleT<M>, Y, A>> f) =>
        new ChronicleT<Y, M, A>(ty => SemigroupInstance<X>.Instance switch
                                      {
                                          { IsSome: true, Value: { } tx } =>
                                              ma.As2()
                                                .Run(tx)
                                                .Bind(ch => ch switch
                                                            {
                                                                These<X, A>.This (var x) =>
                                                                    f(x).As2().Run(ty),

                                                                These<X, A>.That (var a) => M.Pure(
                                                                    That<Y, A>(a)),

                                                                These<X, A>.Both (var x, var a) =>
                                                                    f(x).As2().Run(ty).Map(ch1 =>
                                                                                ch1 switch
                                                                                {
                                                                                    These<Y, A>.This (var y1) =>
                                                                                        Both(y1, a),

                                                                                    These<Y, A>.That (var a1) =>
                                                                                        That<Y, A>(a1),

                                                                                    These<Y, A>.Both (var y1, var a1) =>
                                                                                        Both(y1, a1),

                                                                                    _ => throw new NSE()
                                                                                }),
                                                                _ => throw new NSE()
                                                            }),

                                          _ => throw new NSE($"Type {typeof(X).Name} is not a valid semigroup")
                                      });

    static K<ChronicleT<M>, X, B> Bimonad<ChronicleT<M>>.BindSecond<X, A, B>(
        K<ChronicleT<M>, X, A> ma,
        Func<A, K<ChronicleT<M>, X, B>> f) =>
        new ChronicleT<X, M, B>(
            tx => ma.As2()
                    .Run(tx)
                    .Bind(ch => ch switch
                                {
                                    These<X, A>.This (var x) =>
                                        M.Pure(This<X, B>(x)),

                                    These<X, A>.That (var a) => 
                                        f(a).As2().Run(tx),
                                                      
                                    These<X, A>.Both (var x, var a) =>
                                        f(a).As2().Run(tx).Map(ch1 =>
                                                                   ch1 switch
                                                                   {
                                                                       These<X, B>.This (var x1) =>
                                                                           This<X, B>(tx.Combine(x, x1)),

                                                                       These<X, B>.That (var b1) =>
                                                                           Both(x, b1),

                                                                       These<X, B>.Both (var x1, var b1) =>
                                                                           Both(tx.Combine(x, x1), b1),

                                                                       _ => throw new NSE()
                                                                   }),
                                    _ => throw new NSE()
                                }));
}
