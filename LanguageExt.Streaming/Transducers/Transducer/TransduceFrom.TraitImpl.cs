using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class TransduceFrom<IN> : 
    Monad<TransduceFrom<IN>>,
    Readable<TransduceFrom<IN>, IN>
{
    static K<TransduceFrom<IN>, B> Monad<TransduceFrom<IN>>.Bind<A, B>(
        K<TransduceFrom<IN>, A> ma,
        Func<A, K<TransduceFrom<IN>, B>> f) =>
        ma.As().Bind(f);

    static K<TransduceFrom<IN>, B> Functor<TransduceFrom<IN>>.Map<A, B>(
        Func<A, B> f,
        K<TransduceFrom<IN>, A> ma) =>
        ma.As().Map(f);

    static K<TransduceFrom<IN>, A> Applicative<TransduceFrom<IN>>.Pure<A>(A value) =>
        Transducer.constant<IN, A>(value);

    static K<TransduceFrom<IN>, B> Applicative<TransduceFrom<IN>>.Apply<A, B>(
        K<TransduceFrom<IN>, Func<A, B>> mf,
        K<TransduceFrom<IN>, A> ma) =>
        mf.Bind(ma.Map);

    static K<TransduceFrom<IN>, B> Applicative<TransduceFrom<IN>>.Apply<A, B>(
        K<TransduceFrom<IN>, Func<A, B>> mf,
        Memo<TransduceFrom<IN>, A> ma) =>
        mf.Bind(ma.Map);
    
    public static K<TransduceFrom<IN>, A> Asks<A>(Func<IN, A> f) => 
        Transducer.map(f);

    public static K<TransduceFrom<IN>, A> Local<A>(Func<IN, IN> f, K<TransduceFrom<IN>, A> ma) =>
        ma.As().Comap(f);
}
