using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Traits;

namespace LanguageExt;

public class Eff : 
    Monad<Eff>,
    Fallible<Eff>,
    Stateful<Eff, MinRT>,
    Alternative<Eff>
{
    static K<Eff, B> Monad<Eff>.Bind<A, B>(K<Eff, A> ma, Func<A, K<Eff, B>> f) =>
        ma.As().Bind(f);

    static K<Eff, B> Functor<Eff>.Map<A, B>(Func<A, B> f, K<Eff, A> ma) => 
        ma.As().Map(f);

    static K<Eff, A> Applicative<Eff>.Pure<A>(A value) => 
        Eff<A>.Pure(value);

    static K<Eff, B> Applicative<Eff>.Apply<A, B>(K<Eff, Func<A, B>> mf, K<Eff, A> ma) => 
        mf.As().Apply(ma.As());

    static K<Eff, B> Applicative<Eff>.Action<A, B>(K<Eff, A> ma, K<Eff, B> mb) => 
        ma.As().Action(mb.As());

    static K<Eff, A> MonoidK<Eff>.Empty<A>() => 
        Eff<A>.Fail(Errors.None);

    static K<Eff, A> SemigroupK<Eff>.Combine<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        ma.As() | mb.As();

    static K<Eff, Unit> Stateful<Eff, MinRT>.Put(MinRT value) => 
        new Eff<Unit>(Stateful.put<Eff<MinRT>, MinRT>(value).As());

    static K<Eff, Unit> Stateful<Eff, MinRT>.Modify(Func<MinRT, MinRT> modify) => 
        new Eff<Unit>(Stateful.modify<Eff<MinRT>, MinRT>(modify).As());

    static K<Eff, A> Stateful<Eff, MinRT>.Gets<A>(Func<MinRT, A> f) => 
        new Eff<A>(Stateful.gets<Eff<MinRT>, MinRT, A>(f).As());

    static K<Eff, A> Monad<Eff>.LiftIO<A>(IO<A> ma) =>
        Eff<A>.LiftIO(ma);

    static K<Eff, B> Monad<Eff>.WithRunInIO<A, B>(Func<Func<K<Eff, A>, IO<A>>, IO<B>> inner) =>
        Eff<B>.LiftIO(
            env => inner(ma => ma.As()
                                 .effect
                                 .effect
                                 .Run(env).As()
                                 .Map(p => p.Value)));

    static K<Eff, A> Fallible<Error, Eff>.Fail<A>(Error error) =>
        Eff<A>.Fail(error);

    static K<Eff, A> Fallible<Error, Eff>.Catch<A>(K<Eff, A> fa, Func<Error, bool> Predicate,
                                                   Func<Error, K<Eff, A>> Fail) =>
        fa.As().IfFailEff(e => Predicate(e) ? Fail(e).As() : Eff<A>.Fail(e));
}
