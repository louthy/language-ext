using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where S : TokenStream<S, T>
    where M : Monad<M>
{
    
}
