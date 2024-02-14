using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

/*
public class IO<E> : Monad<IO<E>>, Alternative<IO<E>> 
{
    public static K<IO<E>, B> Bind<A, B>(K<IO<E>, A> ma, Func<A, K<IO<E>, B>> f) => 
        ma.As().Bind(f);

    public static K<IO<E>, B> Map<A, B>(Func<A, B> f, K<IO<E>, A> ma) => 
        ma.As().Map(f);

    public static K<IO<E>, A> Pure<A>(A value) => 
        IO<E, A>.Pure(value);

    public static K<IO<E>, B> Apply<A, B>(K<IO<E>, Func<A, B>> mf, K<IO<E>, A> ma) => 
        mf.As().Apply(ma.As());

    public static K<IO<E>, B> Action<A, B>(K<IO<E>, A> ma, K<IO<E>, B> mb) => 
        ma.As().Action(mb.As());

    public static K<IO<E>, A> Empty<A>() => 
        IO<E, A>.Lift(Transducer.fail<MinRT<E>, Sum<E, A>>(Errors.Bottom));

    public static K<IO<E>, A> Or<A>(K<IO<E>, A> ma, K<IO<E>, A> mb) => 
        ma.As() | mb.As();
    
    public class Runtime<RT> : Monad<Runtime<RT>>, Alternative<Runtime<RT>> 
        where RT : HasIO<RT, E>
    {
        public static K<Runtime<RT>, B> Bind<A, B>(K<Runtime<RT>, A> ma, Func<A, K<Runtime<RT>, B>> f) => 
            ma.As().Bind(f);

        public static K<Runtime<RT>, B> Map<A, B>(Func<A, B> f, K<Runtime<RT>, A> ma) => 
            ma.As().Map(f);

        public static K<Runtime<RT>, A> Pure<A>(A value) => 
            IO<RT, E, A>.Pure(value);

        public static K<Runtime<RT>, B> Apply<A, B>(K<Runtime<RT>, Func<A, B>> mf, K<Runtime<RT>, A> ma) => 
            mf.As().Apply(ma.As());

        public static K<Runtime<RT>, B> Action<A, B>(K<Runtime<RT>, A> ma, K<Runtime<RT>, B> mb) => 
            ma.As().Action(mb.As());

        public static K<Runtime<RT>, A> Empty<A>() => 
            IO<RT, E, A>.Lift(Transducer.fail<RT, Sum<E, A>>(Errors.Bottom));

        public static K<Runtime<RT>, A> Or<A>(K<Runtime<RT>, A> ma, K<Runtime<RT>, A> mb) => 
            ma.As() | mb.As();
    }
}
*/
