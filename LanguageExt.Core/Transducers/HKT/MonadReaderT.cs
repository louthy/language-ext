using System;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface MonadReaderT<MRdr, Env, M> : MonadT<MRdr, M> 
    where M : Monad<M>
    where MRdr : MonadReaderT<MRdr, Env, M>
{
    public static abstract MonadT<MRdr1, M, A> With<MRdr1, Env1, A>(
        Transducer<Env1, Env> f, 
        MonadT<MRdr, M, A> ma) where MRdr1 : MonadReaderT<MRdr1, Env1, M>;

    public static abstract MonadT<MRdr, M, A> Asks<A>(Transducer<Env, A> f);

    public static virtual MonadT<MRdr, M,Env> Ask =>
        MRdr.Asks(identity<Env>());

    public static virtual MonadT<MRdr, M, A> Local<A>(
        Transducer<Env, Env> f,
        MonadT<MRdr, M, A> ma) =>
        MRdr.With(f, ma);

    public static virtual MonadT<MRdr, M, A> Local<A>(
        Func<Env, Env> f,
        MonadT<MRdr, M, A> ma) =>
        MRdr.Local(Prelude.lift(f), ma);
}
