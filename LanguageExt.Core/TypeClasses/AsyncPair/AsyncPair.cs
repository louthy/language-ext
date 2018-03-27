
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public interface AsyncPair<SyncA, AsyncA> : Typeclass
    {
        AsyncA ToAsync(SyncA sa);
    }
}
