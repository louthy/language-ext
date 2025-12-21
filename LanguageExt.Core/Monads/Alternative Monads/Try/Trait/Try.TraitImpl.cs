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
    Final<Try>,
    Alternative<Try>
{
    static K<Try, B> Monad<Try>.Bind<A, B>(K<Try, A> ma, Func<A, K<Try, B>> f) =>
        new Try<B>(() => ma.Run() switch
                         {
                             Fin<A>.Succ(var x) => f(x).Run(),
                             Fin<A>.Fail(var e) => Fin.Fail<B>(e),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, B> Monad<Try>.Recur<A, B>(A value, Func<A, K<Try, Next<A, B>>> f) =>
        lift(() =>
             {
                 while (true)
                 {
                     var mr = +f(value).Run();
                     if (mr.IsFail) return Fin.Fail<B>(mr.FailValue);
                     var next = (Next<A, B>)mr;
                     if (next.IsDone) return Fin.Succ<B>(next.DoneValue);
                     value = next.ContValue;
                 }
             });

    static K<Try, B> Functor<Try>.Map<A, B>(Func<A, B> f, K<Try, A> ma) => 
        new Try<B>(() => ma.Run().Map(f));

    static K<Try, A> Applicative<Try>.Pure<A>(A value) => 
        Succ(value);

    static K<Try, B> Applicative<Try>.Apply<A, B>(K<Try, Func<A, B>> mf, K<Try, A> ma) =>
        new Try<B>(() => mf.Run().Apply(ma.Run()));

    static K<Try, B> Applicative<Try>.Apply<A, B>(K<Try, Func<A, B>> mf, Memo<Try, A> ma) =>
        new Try<B>(() => mf.Run().Apply(ma.Value.Run()));

    static K<Try, A> Alternative<Try>.Empty<A>() =>
        Try.Fail<A>(Error.Empty);

    static K<Try, A> Choice<Try>.Choose<A>(K<Try, A> ma, K<Try, A> mb) =>
        new Try<A>(() => ma.Run() switch
                         {
                             Fin<A>.Succ(var x) => Fin.Succ<A>(x),
                             Fin<A>.Fail        => mb.Run(),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, A> Choice<Try>.Choose<A>(K<Try, A> ma, Memo<Try, A> mb) => 
        new Try<A>(() => ma.Run() switch
                         {
                             Fin<A>.Succ(var x) => Fin.Succ(x),
                             Fin<A>.Fail        => mb.Value.Run(),
                             _                  => throw new NotSupportedException()
                         });

    static K<Try, A> Fallible<Error, Try>.Fail<A>(Error value) => 
        Fail<A>(value);

    static K<Try, A> Fallible<Error, Try>.Catch<A>(
        K<Try, A> fa, 
        Func<Error, bool> Predicate,
        Func<Error, K<Try, A>> Fail) =>
        new Try<A>(() => fa.Run() switch
                         {
                             Fin<A>.Succ ma                       => ma,
                             Fin<A>.Fail(var e) when Predicate(e) => Fail(e).Run(),
                             var ma                               => ma
                         });

    static K<Try, A> Final<Try>.Finally<X, A>(K<Try, A> fa, K<Try, X> @finally) =>
        new Try<A>(() =>
                   {
                       try
                       {
                           return fa.Run();
                       }
                       finally
                       {
                           @finally.Run().ThrowIfFail();
                       }
                   });
}
