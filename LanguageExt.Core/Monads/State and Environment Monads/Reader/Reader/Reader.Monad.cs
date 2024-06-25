using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Trait implementation for `Reader` 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
public partial class Reader<Env> : 
    Monad<Reader<Env>>,
    Readable<Reader<Env>, Env> 
{
    static K<Reader<Env>, B> Monad<Reader<Env>>.Bind<A, B>(K<Reader<Env>, A> ma, Func<A, K<Reader<Env>, B>> f) => 
        ma.As().Bind(f);

    static K<Reader<Env>, B> Functor<Reader<Env>>.Map<A, B>(Func<A, B> f, K<Reader<Env>, A> ma) => 
        ma.As().Map(f);

    static K<Reader<Env>, A> Applicative<Reader<Env>>.Pure<A>(A value) => 
        Reader<Env, A>.Pure(value);

    static K<Reader<Env>, B> Applicative<Reader<Env>>.Apply<A, B>(K<Reader<Env>, Func<A, B>> mf, K<Reader<Env>, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<Reader<Env>, B> Applicative<Reader<Env>>.Action<A, B>(K<Reader<Env>, A> ma, K<Reader<Env>, B> mb) =>
        ma.As().Bind(_ => mb);

    static K<Reader<Env>, Env> Readable<Reader<Env>, Env>.Ask =>
        Reader<Env, Env>.Asks(Prelude.identity);

    static K<Reader<Env>, A> Readable<Reader<Env>, Env>.Asks<A>(Func<Env, A> f) => 
        Reader<Env, A>.Asks(f);

    static K<Reader<Env>, A> Readable<Reader<Env>, Env>.Local<A>(Func<Env, Env> f, K<Reader<Env>, A> ma) =>
        ma.As().Local(f);
}
