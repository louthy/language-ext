using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public class StreamT<M> : 
    MonadT<StreamT<M>, M>,
    Alternative<StreamT<M>>
    where M : Monad<M>, Alternative<M>
{
    static K<StreamT<M>, B> Monad<StreamT<M>>.Bind<A, B>(K<StreamT<M>, A> ma, Func<A, K<StreamT<M>, B>> f) =>
        ma.As().Bind(f);

    static K<StreamT<M>, B> Functor<StreamT<M>>.Map<A, B>(Func<A, B> f, K<StreamT<M>, A> ma) => 
        ma.As().Map(f);

    static K<StreamT<M>, A> Applicative<StreamT<M>>.Pure<A>(A value) => 
        StreamT.pure<M, A>(value);

    static K<StreamT<M>, B> Applicative<StreamT<M>>.Apply<A, B>(K<StreamT<M>, Func<A, B>> mf, K<StreamT<M>, A> ma) =>
        new StreamT<M, B>(ma.As().runStreamT.Bind(a => mf.As().runStreamT.Map(f => f.Apply(a))));

    static K<StreamT<M>, A> MonadT<StreamT<M>, M>.Lift<A>(K<M, A> ma) => 
        StreamT.liftM(ma);

    static K<StreamT<M>, A> Choice<StreamT<M>>.Choose<A>(K<StreamT<M>, A> fa, K<StreamT<M>, A> fb) => 
        throw new NotImplementedException();

    static K<StreamT<M>, A> SemigroupK<StreamT<M>>.Combine<A>(K<StreamT<M>, A> lhs, K<StreamT<M>, A> rhs) => 
        throw new NotImplementedException();

    static K<StreamT<M>, A> MonoidK<StreamT<M>>.Empty<A>() => 
        throw new NotImplementedException();
}
