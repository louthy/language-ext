using LanguageExt.Traits;

namespace LanguageExt.ClassInstances.Pred;

public struct NonNullItems<A> : Pred<A?>
{
    public static bool True(A? value) => 
        !value.IsNull();
}
