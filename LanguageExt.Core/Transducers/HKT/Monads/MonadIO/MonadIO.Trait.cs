using LanguageExt.Common;

namespace LanguageExt.HKT;

public interface MonadIO<M> : Monad<M> 
    where M : Monad<M>
{
    public static abstract K<M, A> LiftIO<A>(IO<A> ma);

    public static virtual K<M, A> LiftNoIO<A>(IO<A> ma) =>
        throw new ExceptionalException(Errors.IONotInTransformerStackText, Errors.IONotInTransformerStackCode);
}
