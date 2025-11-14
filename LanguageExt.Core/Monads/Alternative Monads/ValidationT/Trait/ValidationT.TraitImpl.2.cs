using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using NSE = System.NotSupportedException;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `ValidationT` 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class ValidationT<M> :  
    CoproductK<ValidationT<M>>,
    Bimonad<ValidationT<M>> 
    where M : Monad<M>
{
    static K<ValidationT<M>, F, A> CoproductCons<ValidationT<M>>.Left<F, A>(F value) => 
        ValidationT.FailI<F, M, A>(value);

    static K<ValidationT<M>, F, A> CoproductCons<ValidationT<M>>.Right<F, A>(A value) => 
        ValidationT.SuccessI<F, M, A>(value);

    static K<ValidationT<M>, F, B> CoproductK<ValidationT<M>>.Match<F, A, B>(
        Func<F, B> Left, 
        Func<A, B> Right, 
        K<ValidationT<M>, F, A> fab) => 
        new ValidationT<F, M, B>(monoid => fab.As2().MatchI(Left, Right)(monoid).Map(Validation.SuccessI<F, B>));

    static K<ValidationT<M>, F2, B> Bifunctor<ValidationT<M>>.BiMap<F1, A, F2, B>(
        Func<F1, F2> first,
        Func<A, B> second,
        K<ValidationT<M>, F1, A> fab) =>
        new ValidationT<F2, M, B>(_ => MonoidInstance<F1>.Instance switch
                                       {
                                           { IsSome: true, Value: { } t } =>
                                               fab.As2()
                                                  .Run(t)
                                                  .Map(v => v switch
                                                            {
                                                                Validation<F1, A>.Fail(var e) =>
                                                                    Validation.FailI<F2, B>(first(e)),

                                                                Validation<F1, A>.Success(var x) =>
                                                                    Validation.SuccessI<F2, B>(second(x)),

                                                                _ => throw new NSE()
                                                            }),

                                           _ => throw new NSE($"Type {typeof(F1).Name} is not a valid monoid")
                                       });

    static K<ValidationT<M>, F2, A> Bimonad<ValidationT<M>>.BindFirst<F1, F2, A>(
        K<ValidationT<M>, F1, A> ma,
        Func<F1, K<ValidationT<M>, F2, A>> f) =>
        new ValidationT<F2, M, A>(ty => MonoidInstance<F1>.Instance switch
                                       {
                                           { IsSome: true, Value: { } tx } =>
                                               ma.As2()
                                                 .Run(tx)
                                                 .Bind(v => v switch
                                                            {
                                                                Validation<F1, A>.Fail (var e) =>
                                                                    f(e).As2().Run(ty),

                                                                Validation<F1, A>.Success (var x) =>
                                                                    M.Pure(Validation.SuccessI<F2, A>(x)),

                                                                _ => throw new NSE()
                                                            }),

                                           _ => throw new NSE($"Type {typeof(F1).Name} is not a valid monoid")
                                       });

    static K<ValidationT<M>, F, B> Bimonad<ValidationT<M>>.BindSecond<F, A, B>(
        K<ValidationT<M>, F, A> ma, 
        Func<A, K<ValidationT<M>, F, B>> f) =>
        new ValidationT<F, M, B>(ty => MonoidInstance<F>.Instance switch
                                       {
                                           { IsSome: true, Value: { } tx } =>
                                               ma.As2()
                                                 .Run(tx)
                                                 .Bind(v => v switch
                                                            {
                                                                Validation<F, A>.Fail (var e) =>
                                                                    M.Pure(Validation.FailI<F, B>(e)),

                                                                Validation<F, A>.Success (var x) =>
                                                                    f(x).As2().Run(ty),

                                                                _ => throw new NSE()
                                                            }),

                                           _ => throw new NSE($"Type {typeof(F).Name} is not a valid monoid")
                                       });
}
