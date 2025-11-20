using LanguageExt.Traits;

namespace LanguageExt.Pipes;

class PipeTCached<IN, OUT, M>
    where M : MonadIO<M>
{
    public static readonly PipeT<IN, OUT, M, Unit> unitP = 
        PipeT.pure<IN, OUT, M, Unit>(default);
}
