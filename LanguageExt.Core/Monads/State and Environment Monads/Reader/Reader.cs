using LanguageExt.HKT;

namespace LanguageExt;

//public delegate Fin<A> Reader<in Env, A>(Env env);

public static class Reader
{
    public static Reader<Env, A> As<Env, A>(this MonadT<ReaderT<Env, MIdentity>, MIdentity, A> ma) =>
        (Reader<Env, A>)ma;
}

public record Reader<Env, A>(Transducer<Env, Monad<MIdentity, A>> RunReader)
    : ReaderT<Env, MIdentity, A>(RunReader);
