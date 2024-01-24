using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public struct MUnit : Monoid<Unit>
{
    public static Unit Append(Unit x, Unit y) =>
        unit;

    public static Unit Empty() =>
        unit;
}
