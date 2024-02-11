using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.HKT;

namespace LanguageExt;

public class MIO<E> : Monad<MIO<E>>, Alternative<MIO<E>> 
{
    public static K<MIO<E>, B> Bind<A, B>(K<MIO<E>, A> ma, Func<A, K<MIO<E>, B>> f) => 
        ma.As().Bind(f);

    public static K<MIO<E>, B> Map<A, B>(Func<A, B> f, K<MIO<E>, A> ma) => 
        ma.As().Map(f);

    public static K<MIO<E>, A> Pure<A>(A value) => 
        IO<E, A>.Pure(value);

    public static K<MIO<E>, B> Apply<A, B>(K<MIO<E>, Func<A, B>> mf, K<MIO<E>, A> ma) => 
        mf.As().Apply(ma.As());

    public static K<MIO<E>, B> Action<A, B>(K<MIO<E>, A> ma, K<MIO<E>, B> mb) => 
        ma.As().Action(mb.As());

    public static K<MIO<E>, A> Empty<A>() => 
        IO<E, A>.Lift(Transducer.fail<MinRT<E>, Sum<E, A>>(Errors.Bottom));

    public static K<MIO<E>, A> Or<A>(K<MIO<E>, A> ma, K<MIO<E>, A> mb) => 
        ma.As() | mb.As();
}
