using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `Try` 
/// </summary>
public partial class Try : Monad<Try>, SemiAlternative<Try>
{
    static K<Try, B> Monad<Try>.Bind<A, B>(K<Try, A> ma, Func<A, K<Try, B>> f) => 
        ma.As().Bind(f);

    static K<Try, B> Functor<Try>.Map<A, B>(Func<A, B> f, K<Try, A> ma) => 
        ma.As().Map(f);

    static K<Try, A> Applicative<Try>.Pure<A>(A value) => 
        Try<A>.Succ(value);

    static K<Try, B> Applicative<Try>.Apply<A, B>(K<Try, Func<A, B>> mf, K<Try, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<Try, B> Applicative<Try>.Action<A, B>(K<Try, A> ma, K<Try, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<Try, A> SemigroupK<Try>.Combine<A>(K<Try, A> ma, K<Try, A> mb) =>
        ma.As().Combine(mb.As());
}
