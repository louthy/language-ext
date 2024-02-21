using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

public class Eff : 
    Monad<Eff>,
    State<Eff, MinRT>,
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

    static K<Eff, A> Alternative<Eff>.Or<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        ma.As() | mb.As();

    // static K<Eff, A> Reader<Eff, MinRT>.Asks<A>(Func<MinRT, A> f) =>
    //     Eff<A>.Lift(f);
    //
    // static K<Eff, A> Reader<Eff, MinRT>.Local<A>(Func<MinRT, MinRT> f, K<Eff, A> ma) =>
    //     new Eff<A>(ma.As().effect.With(f));

    static K<Eff, A> Resource<Eff>.Use<A>(IO<A> ma, Func<A, IO<Unit>> release) => 
        new Eff<A>(new Eff<MinRT, A>(StateT<MinRT>.lift(ResourceT<IO>.use(ma, release))));

    static K<Eff, Unit> Resource<Eff>.Release<A>(A value) =>
        new Eff<Unit>(new Eff<MinRT, Unit>(StateT<MinRT>.lift(ResourceT<IO>.release(value))));
    
    public class R<RT> : 
        State<R<RT>, RT>, 
        Resource<R<RT>>,
        Alternative<R<RT>>, 
        Monad<R<RT>>
        where RT : HasIO<RT>
    {
        static K<R<RT>, B> Monad<R<RT>>.Bind<A, B>(K<R<RT>, A> ma, Func<A, K<R<RT>, B>> f) => 
            ma.As().Bind(f);

        static K<R<RT>, B> Functor<R<RT>>.Map<A, B>(Func<A, B> f, K<R<RT>, A> ma) => 
            ma.As().Map(f);

        static K<R<RT>, A> Applicative<R<RT>>.Pure<A>(A value) => 
            Eff<RT, A>.Pure(value);

        static K<R<RT>, B> Applicative<R<RT>>.Apply<A, B>(K<R<RT>, Func<A, B>> mf, K<R<RT>, A> ma) => 
            mf.As().Apply(ma.As());

        static K<R<RT>, B> Applicative<R<RT>>.Action<A, B>(K<R<RT>, A> ma, K<R<RT>, B> mb) => 
            ma.As().Action(mb.As());

        static K<R<RT>, A> Alternative<R<RT>>.Empty<A>() => 
            Eff<RT, A>.Fail(Errors.None);

        static K<R<RT>, A> Alternative<R<RT>>.Or<A>(K<R<RT>, A> ma, K<R<RT>, A> mb) => 
            ma.As() | mb.As();

        /*
        static K<R<RT>, A> Reader<R<RT>, RT>.Asks<A>(Func<RT, A> f) => 
            Eff<RT, A>.Lift(f);

        static K<R<RT>, A> Reader<R<RT>, RT>.Local<A>(Func<RT, RT> f, K<R<RT>, A> ma) => 
            ma.As().With(f);
            */

        static K<R<RT>, A> Resource<R<RT>>.Use<A>(IO<A> ma, Func<A, IO<Unit>> release) =>
            new Eff<RT, A>(StateT<RT>.lift(ResourceT<IO>.use(ma, release)));

        static K<R<RT>, Unit> Resource<R<RT>>.Release<A>(A value) => 
            new Eff<RT, Unit>(StateT<RT>.lift(ResourceT<IO>.release(value)));

        static K<R<RT>, Unit> State<R<RT>, RT>.Put(RT value) =>
            new Eff<RT, Unit>(StateT.put<RT, ResourceT<IO>>(value));

        static K<R<RT>, Unit> State<R<RT>, RT>.Modify(Func<RT, RT> modify) => 
            new Eff<RT, Unit>(StateT.modify<RT, ResourceT<IO>>(modify));

        static K<R<RT>, A> State<R<RT>, RT>.Gets<A>(Func<RT, A> f) => 
            new Eff<RT, A>(StateT.gets<RT, ResourceT<IO>, A>(f));
    }

    static K<Eff, Unit> State<Eff, MinRT>.Put(MinRT value) => 
        new Eff<Unit>(State.put<R<MinRT>, MinRT>(value).As());

    static K<Eff, Unit> State<Eff, MinRT>.Modify(Func<MinRT, MinRT> modify) => 
        new Eff<Unit>(State.modify<R<MinRT>, MinRT>(modify).As());

    static K<Eff, A> State<Eff, MinRT>.Gets<A>(Func<MinRT, A> f) => 
        new Eff<A>(State.gets<R<MinRT>, MinRT, A>(f).As());
}
