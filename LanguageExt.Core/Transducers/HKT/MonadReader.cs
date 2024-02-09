using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface MonadReader<M, E> : Functor<M>
    where M : MonadReader<M, E>;
