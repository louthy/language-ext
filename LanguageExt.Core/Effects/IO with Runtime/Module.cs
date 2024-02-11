using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.HKT;

namespace LanguageExt;

public class MIO<RT, E> : Monad<MIO<RT, E>>, Alternative<MIO<RT, E>> 
    where RT : HasIO<RT, E>
{
    public static K<MIO<RT, E>, B> Bind<A, B>(K<MIO<RT, E>, A> ma, Func<A, K<MIO<RT, E>, B>> f) => 
        ma.As().Bind(f);

    public static K<MIO<RT, E>, B> Map<A, B>(Func<A, B> f, K<MIO<RT, E>, A> ma) => 
        ma.As().Map(f);

    public static K<MIO<RT, E>, A> Pure<A>(A value) => 
        IO<RT, E, A>.Pure(value);

    public static K<MIO<RT, E>, B> Apply<A, B>(K<MIO<RT, E>, Func<A, B>> mf, K<MIO<RT, E>, A> ma) => 
        mf.As().Apply(ma.As());

    public static K<MIO<RT, E>, B> Action<A, B>(K<MIO<RT, E>, A> ma, K<MIO<RT, E>, B> mb) => 
        ma.As().Action(mb.As());

    public static K<MIO<RT, E>, A> Empty<A>() => 
        IO<RT, E, A>.Lift(Transducer.fail<RT, Sum<E, A>>(Errors.Bottom));

    public static K<MIO<RT, E>, A> Or<A>(K<MIO<RT, E>, A> ma, K<MIO<RT, E>, A> mb) => 
        ma.As() | mb.As();
}
