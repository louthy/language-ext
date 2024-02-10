using System;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface MonadReaderT<MRdr, Env, M> : MonadT<MRdr, M> 
    where M : Monad<M>
    where MRdr : MonadReaderT<MRdr, Env, M>
{
    public static abstract MonadReaderT<MRdr, Env, M, A> Asks<A>(Transducer<Env, A> f);

    public static virtual MonadReaderT<MRdr, Env, M, Env> Ask =>
        MRdr.Asks(identity<Env>());

    public static abstract MonadReaderT<MRdr, Env, M, A> Local<A>(
        Transducer<Env, Env> f,
        MonadT<MRdr, M, A> ma);

    public static virtual MonadReaderT<MRdr, Env, M, A> Local<A>(
        Func<Env, Env> f,
        MonadT<MRdr, M, A> ma) =>
        MRdr.Local(Prelude.lift(f), ma);
}
