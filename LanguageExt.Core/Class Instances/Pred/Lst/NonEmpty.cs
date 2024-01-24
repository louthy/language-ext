using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances.Pred;

/// <summary>
/// Lst must be non-empty
/// </summary>
public struct NonEmpty : Pred<ListInfo>
{
    public static bool True(ListInfo value) =>
        value.Count != 0;
}
