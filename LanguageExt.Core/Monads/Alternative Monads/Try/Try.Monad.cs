using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `Try` 
/// </summary>
public partial class Try : 
    Monad<Try>,
    Fallible<Try>, 
    Alternative<Try>
{
    static K<Try, B> Monad<Try>.Bind<A, B>(K<Try, A> ma, Func<A, K<Try, B>> f) =>
        new Try<B>(() => ma.Run() switch
                         {
                             Fin.Succ<A>(var x) => f(x).Run(),
                             Fin.Fail<A>(var e) => Fin<B>.Fail(e),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, B> Functor<Try>.Map<A, B>(Func<A, B> f, K<Try, A> ma) => 
        new Try<B>(() => ma.Run().Map(f));

    static K<Try, A> Applicative<Try>.Pure<A>(A value) => 
        Try<A>.Succ(value);

    static K<Try, B> Applicative<Try>.Apply<A, B>(K<Try, Func<A, B>> mf, K<Try, A> ma) =>
        new Try<B>(() => mf.Run().Apply(ma.Run()));

    static K<Try, B> Applicative<Try>.Action<A, B>(K<Try, A> ma, K<Try, B> mb) =>
        new Try<B>(() => ma.Run() switch
                         {
                             Fin.Succ<A>        => mb.Run(),
                             Fin.Fail<A>(var e) => Fin<B>.Fail(e),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, A> MonoidK<Try>.Empty<A>() =>
        Try<A>.Fail(Error.Empty);

    static K<Try, A> SemigroupK<Try>.Combine<A>(K<Try, A> ma, K<Try, A> mb) =>
        new Try<A>(() => ma.Run() switch
                         {
                             Fin.Succ<A>(var x) => Fin<A>.Succ(x),
                             Fin.Fail<A> fa     => fa.Combine(mb.Run()).As(),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, A> Choice<Try>.Choose<A>(K<Try, A> ma, K<Try, A> mb) =>
        new Try<A>(() => ma.Run() switch
                         {
                             Fin.Succ<A>(var x) => Fin<A>.Succ(x),
                             Fin.Fail<A>        => mb.Run(),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, A> Choice<Try>.Choose<A>(K<Try, A> ma, Func<K<Try, A>> mb) => 
        new Try<A>(() => ma.Run() switch
                         {
                             Fin.Succ<A>(var x) => Fin<A>.Succ(x),
                             Fin.Fail<A>        => mb().Run(),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, A> Fallible<Error, Try>.Fail<A>(Error value) => 
        Try<A>.Fail(value);

    static K<Try, A> Fallible<Error, Try>.Catch<A>(
        K<Try, A> fa, 
        Func<Error, bool> Predicate,
        Func<Error, K<Try, A>> Fail) =>
        new Try<A>(() => fa.Run() switch
                         {
                             Fin.Succ<A> ma                       => ma,
                             Fin.Fail<A>(var e) when Predicate(e) => Fail(e).Run(),
                             var ma                               => ma
                         });
}
