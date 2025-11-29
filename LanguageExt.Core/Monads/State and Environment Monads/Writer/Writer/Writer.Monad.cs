using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `MonadStateT` trait implementation for `StateT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class Writer<W> : 
    Monad<Writer<W>>, 
    Writable<Writer<W>, W>
    where W : Monoid<W>
{
    static K<Writer<W>, B> Monad<Writer<W>>.Bind<A, B>(K<Writer<W>, A> ma, Func<A, K<Writer<W>, B>> f) => 
        ma.As().Bind(f);

    static K<Writer<W>, B> Functor<Writer<W>>.Map<A, B>(Func<A, B> f, K<Writer<W>, A> ma) => 
        ma.As().Map(f);

    static K<Writer<W>, A> Applicative<Writer<W>>.Pure<A>(A value) => 
        Writer<W, A>.Pure(value);

    static K<Writer<W>, B> Applicative<Writer<W>>.Apply<A, B>(K<Writer<W>, Func<A, B>> mf, K<Writer<W>, A> ma) => 
        mf.As().Bind(x => ma.As().Map(x));

    static K<Writer<W>, B> Applicative<Writer<W>>.Apply<A, B>(K<Writer<W>, Func<A, B>> mf, Memo<Writer<W>, A> ma) => 
        mf.As().Bind(x => ma.Value.As().Map(x));

    static K<Writer<W>, Unit> Writable<Writer<W>, W>.Tell(W item) =>
        new Writer<W, Unit>(w => (unit, w + item));

    static K<Writer<W>, (A Value, W Output)> Writable<Writer<W>, W>.Listen<A>(K<Writer<W>, A> ma) =>
        ma.As().Listen();

    static K<Writer<W>, A> Writable<Writer<W>, W>.Pass<A>(
        K<Writer<W>, (A Value, Func<W, W> Function)> action) =>
        new Writer<W, A>(
            w =>
            {
                var ((a, f), w1) = action.As().Run();
                return (a, w + f(w1));
            });
}
