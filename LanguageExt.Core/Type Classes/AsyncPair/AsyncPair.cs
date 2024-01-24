#nullable enable

using LanguageExt.Attributes;
using LanguageExt.TypeClasses;

namespace LanguageExt;

[Trait("AsyncPair*")]
public interface AsyncPair<in SyncA, out AsyncA> : Trait
{
    public static abstract AsyncA ToAsync(SyncA sa);
}
