using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;

namespace LanguageExt.ClassInstances
{
    public struct MUnit : Monoid<Unit>
    {
        public Unit Append(Unit x, Unit y) =>
            unit;

        public Unit Empty() =>
            unit;
    }
}
