using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

public class Eff : Monad<Eff>, Alternative<Eff> 
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
        Eff<A>.Lift(Transducer.fail<MinRT, Sum<Error, A>>(Errors.None));

    static K<Eff, A> Alternative<Eff>.Or<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        ma.As() | mb.As();
    
    public class Runtime<RT> : Monad<Runtime<RT>>, Alternative<Runtime<RT>> 
        where RT : HasIO<RT, Error>
    {
        static K<Runtime<RT>, B> Monad<Runtime<RT>>.Bind<A, B>(K<Runtime<RT>, A> ma, Func<A, K<Runtime<RT>, B>> f) => 
            ma.As().Bind(f);

        static K<Runtime<RT>, B> Functor<Runtime<RT>>.Map<A, B>(Func<A, B> f, K<Runtime<RT>, A> ma) => 
            ma.As().Map(f);

        static K<Runtime<RT>, A> Applicative<Runtime<RT>>.Pure<A>(A value) => 
            Eff<RT, A>.Pure(value);

        static K<Runtime<RT>, B> Applicative<Runtime<RT>>.Apply<A, B>(K<Runtime<RT>, Func<A, B>> mf, K<Runtime<RT>, A> ma) => 
            mf.As().Apply(ma.As());

        static K<Runtime<RT>, B> Applicative<Runtime<RT>>.Action<A, B>(K<Runtime<RT>, A> ma, K<Runtime<RT>, B> mb) => 
            ma.As().Action(mb.As());

        static K<Runtime<RT>, A> Alternative<Runtime<RT>>.Empty<A>() => 
            Eff<RT, A>.Fail(Errors.None);

        static K<Runtime<RT>, A> Alternative<Runtime<RT>>.Or<A>(K<Runtime<RT>, A> ma, K<Runtime<RT>, A> mb) => 
            ma.As() | mb.As();
    }
}
