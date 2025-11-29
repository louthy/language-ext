using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class TransduceFromM<M, IN> : 
    Monad<TransduceFromM<M, IN>>,
    Readable<TransduceFromM<M, IN>, IN>
{
    static K<TransduceFromM<M, IN>, B> Monad<TransduceFromM<M, IN>>.Bind<A, B>(
        K<TransduceFromM<M, IN>, A> ma,
        Func<A, K<TransduceFromM<M, IN>, B>> f) =>
        ma.As().Bind(f);

    static K<TransduceFromM<M, IN>, B> Functor<TransduceFromM<M, IN>>.Map<A, B>(
        Func<A, B> f,
        K<TransduceFromM<M, IN>, A> ma) =>
        ma.As().Map(f);

    static K<TransduceFromM<M, IN>, A> Applicative<TransduceFromM<M, IN>>.Pure<A>(A value) =>
        TransducerM.constant<M, IN, A>(value);

    static K<TransduceFromM<M, IN>, B> Applicative<TransduceFromM<M, IN>>.Apply<A, B>(
        K<TransduceFromM<M, IN>, Func<A, B>> mf,
        K<TransduceFromM<M, IN>, A> ma) =>
        mf.Bind(ma.Map);

    static K<TransduceFromM<M, IN>, B> Applicative<TransduceFromM<M, IN>>.Apply<A, B>(
        K<TransduceFromM<M, IN>, Func<A, B>> mf,
        Memo<TransduceFromM<M, IN>, A> ma) =>
        mf.Bind(ma.Map);

    public static K<TransduceFromM<M, IN>, A> Asks<A>(Func<IN, A> f) => 
        TransducerM.map<M, IN, A>(f);

    public static K<TransduceFromM<M, IN>, A> Local<A>(Func<IN, IN> f, K<TransduceFromM<M, IN>, A> ma) =>
        ma.As().Comap(f);
}
