using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class Transducer<IN> : Monad<Transducer<IN>>
{
    static K<Transducer<IN>, B> Monad<Transducer<IN>>.Bind<A, B>(
        K<Transducer<IN>, A> ma,
        Func<A, K<Transducer<IN>, B>> f) =>
        ma.As().Bind(f);

    static K<Transducer<IN>, B> Functor<Transducer<IN>>.Map<A, B>(
        Func<A, B> f, 
        K<Transducer<IN>, A> ma) => 
        throw new NotImplementedException();

    static K<Transducer<IN>, A> Applicative<Transducer<IN>>.Pure<A>(A value) => 
        throw new NotImplementedException();

    static K<Transducer<IN>, B> Applicative<Transducer<IN>>.Apply<A, B>(
        K<Transducer<IN>, Func<A, B>> mf, 
        K<Transducer<IN>, A> ma) => 
        throw new NotImplementedException();
}
