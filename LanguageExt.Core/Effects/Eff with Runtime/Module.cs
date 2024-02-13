using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.HKT;

namespace LanguageExt;

public class Eff : Monad<Eff>, Alternative<Eff> 
{
    public static K<Eff, B> Bind<A, B>(K<Eff, A> ma, Func<A, K<Eff, B>> f) => 
        ma.As().Bind(f);

    public static K<Eff, B> Map<A, B>(Func<A, B> f, K<Eff, A> ma) => 
        ma.As().Map(f);

    public static K<Eff, A> Pure<A>(A value) => 
        Eff<A>.Pure(value);

    public static K<Eff, B> Apply<A, B>(K<Eff, Func<A, B>> mf, K<Eff, A> ma) => 
        mf.As().Apply(ma.As());

    public static K<Eff, B> Action<A, B>(K<Eff, A> ma, K<Eff, B> mb) => 
        ma.As().Action(mb.As());

    public static K<Eff, A> Empty<A>() => 
        Eff<A>.Lift(Transducer.fail<MinRT, Sum<Error, A>>(Errors.None));

    public static K<Eff, A> Or<A>(K<Eff, A> ma, K<Eff, A> mb) => 
        ma.As() | mb.As();
    
    public class Runtime<RT> : Monad<Runtime<RT>>, Alternative<Runtime<RT>> 
        where RT : HasIO<RT, Error>
    {
        public static K<Runtime<RT>, B> Bind<A, B>(K<Runtime<RT>, A> ma, Func<A, K<Runtime<RT>, B>> f) => 
            ma.As().Bind(f);

        public static K<Runtime<RT>, B> Map<A, B>(Func<A, B> f, K<Runtime<RT>, A> ma) => 
            ma.As().Map(f);

        public static K<Runtime<RT>, A> Pure<A>(A value) => 
            Eff<RT, A>.Pure(value);

        public static K<Runtime<RT>, B> Apply<A, B>(K<Runtime<RT>, Func<A, B>> mf, K<Runtime<RT>, A> ma) => 
            mf.As().Apply(ma.As());

        public static K<Runtime<RT>, B> Action<A, B>(K<Runtime<RT>, A> ma, K<Runtime<RT>, B> mb) => 
            ma.As().Action(mb.As());

        public static K<Runtime<RT>, A> Empty<A>() => 
            Eff<RT, A>.Lift(Transducer.fail<RT, Sum<Error, A>>(Errors.None));

        public static K<Runtime<RT>, A> Or<A>(K<Runtime<RT>, A> ma, K<Runtime<RT>, A> mb) => 
            ma.As() | mb.As();
    }
}
