using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

class PipeTCached<IN, OUT, M>
    where M : Monad<M>
{
    public static readonly PipeT<IN, OUT, M, Unit> unitP = 
        PipeT.pure<IN, OUT, M, Unit>(default);
}
