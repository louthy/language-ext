using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Reader monad
/// </summary>
/// <remarks>
/// This is a composition of the `ReaderT` monad transformer and the `Identity` monad
/// </remarks>
/// <param name="runReader">Transducer that is the reader operation</param>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Reader<Env, A>(Transducer<Env, Monad<MIdentity, A>> runReader)
    : ReaderT<Env, MIdentity, A>(runReader);
