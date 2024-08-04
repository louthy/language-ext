using System;
using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class IO : 
    Monad<IO>, 
    Fallible<IO>,
    Alternative<IO>
{
    public static IO<A> pure<A>(A value) => 
        IO<A>.pure(value);
    
    static K<IO, B> Monad<IO>.Bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma.As().Bind(f);

    static K<IO, B> Functor<IO>.Map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma.As().Map(f);

    static K<IO, A> Applicative<IO>.Pure<A>(A value) => 
        IO<A>.pure(value);

    static K<IO, B> Applicative<IO>.Apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) => 
        mf.As().Bind(ma.As().Map);

    static K<IO, B> Applicative<IO>.Action<A, B>(K<IO, A> ma, K<IO, B> mb) =>
        ma.As().Bind(_ => mb);
    
    static K<IO, A> Applicative<IO>.Actions<A>(IEnumerable<K<IO, A>> fas) =>
        IO<A>.Lift(
            envIO =>
            {
                A? rs = default;
                foreach (var kfa in fas)
                {
                    var fa = kfa.As();
                    rs = fa.Run(envIO);
                }
                if (rs is null) throw new ExceptionalException(Errors.SequenceEmptyText, Errors.SequenceEmptyCode);
                return rs;
            });    

    static K<IO, A> MonoidK<IO>.Empty<A>() =>
        IO<A>.Empty;

    static K<IO, A> SemigroupK<IO>.Combine<A>(K<IO, A> ma, K<IO, A> mb) => 
        ma.As() | mb.As();

    static K<IO, A> Monad<IO>.LiftIO<A>(IO<A> ma) => 
        ma;
    
    static K<IO, B> Monad<IO>.WithRunInIO<A, B>(
        Func<Func<K<IO, A>, IO<A>>, IO<B>> inner) =>
        inner(io => io.As());

    static K<IO, A> Fallible<Error, IO>.Fail<A>(Error error) =>
        fail<A>(error);

    static K<IO, A> Fallible<Error, IO>.Catch<A>(K<IO, A> fa, Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        fa.As().IfFail(e => Predicate(e) ? Fail(e).As() : fail<A>(e));
}
