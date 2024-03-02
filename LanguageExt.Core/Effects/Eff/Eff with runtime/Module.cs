using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Traits;

namespace LanguageExt;

public class Eff : 
    Monad<Eff>,
    StateM<Eff, MinRT>,
    Resource<Eff>,
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

    static K<Eff, A> Alternative<Eff>.Empty<A>() => 
        Eff<A>.Fail(Errors.None);

    static K<Eff, A> SemiAlternative<Eff>.Or<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        ma.As() | mb.As();

    static K<Eff, A> Resource<Eff>.Use<A>(IO<A> ma, Func<A, IO<Unit>> release) => 
        new Eff<A>(new Eff<MinRT, A>(StateT<MinRT>.lift(ResourceT<IO>.use(ma, release))));

    static K<Eff, Unit> Resource<Eff>.Release<A>(A value) =>
        new Eff<Unit>(new Eff<MinRT, Unit>(StateT<MinRT>.lift(ResourceT<IO>.release(value))));

    static K<Eff, Unit> StateM<Eff, MinRT>.Put(MinRT value) => 
        new Eff<Unit>(StateM.put<Eff<MinRT>, MinRT>(value).As());

    static K<Eff, Unit> StateM<Eff, MinRT>.Modify(Func<MinRT, MinRT> modify) => 
        new Eff<Unit>(StateM.modify<Eff<MinRT>, MinRT>(modify).As());

    static K<Eff, A> StateM<Eff, MinRT>.Gets<A>(Func<MinRT, A> f) => 
        new Eff<A>(StateM.gets<Eff<MinRT>, MinRT, A>(f).As());

    static K<Eff, A> Monad<Eff>.LiftIO<A>(IO<A> ma) =>
        Eff<A>.LiftIO(ma);
    
    /*
     TODO -- can get the resources from ResourceT.resources -- consider restoring
    static K<Eff, B> Monad<Eff>.WithRunInIO<A, B>(Func<Func<K<Eff, A>, IO<A>>, IO<B>> inner) =>
        Eff<B>.LiftIO(
            env => inner(ma => ma.As()
                                 .effect
                                 .effect     
                                 .Run(env).As()
                                 .Run().As()
                                 .Map(p => p.Value)));
*/
}
