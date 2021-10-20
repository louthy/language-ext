
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    [Typeclass("AsyncPair*")]
    public interface AsyncPair<SyncA, AsyncA> : Typeclass
    {
        AsyncA ToAsync(SyncA sa);
    }
}
