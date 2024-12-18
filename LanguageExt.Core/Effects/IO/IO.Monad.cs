using System;
using LanguageExt.Common;
using LanguageExt.DSL;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IO : 
    Monad<IO>, 
    Fallible<IO>,
    Alternative<IO>
{
    static K<IO, B> Applicative<IO>.Apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) =>
        ma.As().ApplyBack(mf.As());

    static K<IO, B> Monad<IO>.Bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma.As().Bind(f);

    static K<IO, B> Functor<IO>.Map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma.As().Map(f);

    static K<IO, A> Applicative<IO>.Pure<A>(A value) =>
        new IOPure<A>(value);

    static K<IO, A> Fallible<Error, IO>.Fail<A>(Error error) => 
        IO<A>.Fail(error);

    static K<IO, A> Fallible<Error, IO>.Catch<A>(
        K<IO, A> fa, 
        Func<Error, bool> Predicate,
        Func<Error, K<IO, A>> Fail) =>
        IO<A>.Lift(new IOCatch<A, A>(fa, Predicate, Fail, identity));

    static K<IO, A> Choice<IO>.Choose<A>(K<IO, A> fa, K<IO, A> fb) => 
        IO<A>.Lift(new IOCatch<A, A>(fa, _ => true, _ => fb, identity));

    static K<IO, A> MonoidK<IO>.Empty<A>() =>
        fail<A>(Errors.None);

    static K<IO, A> MonadIO<IO>.LiftIO<A>(IO<A> ma) => 
        ma;

    static K<IO, IO<A>> MonadIO<IO>.ToIO<A>(K<IO, A> ma) => 
        pure(ma.As());

    static K<IO, B> MonadIO<IO>.MapIO<A, B>(K<IO, A> ma, Func<IO<A>, IO<B>> f) =>
        f(ma.As());
}
