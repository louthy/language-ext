using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public partial class ruleFor<A>
    {
        public sealed class All<R1, R2>
            : Rule.All<R1, R2, A>, Rule<All<R1, R2>, A>
            where R1 : Rule<R1, A>, new()
            where R2 : Rule<R2, A>, new();

        public sealed class Any<R1, R2>
            : Rule.Any<R1, R2, A>, Rule<Any<R1, R2>, A>
            where R1 : Rule<R1, A>, new()
            where R2 : Rule<R2, A>, new();
        public sealed class Not<R>
            : Rule.Not<R, A>, Rule<Not<R>, A>
            where R : Rule<R, A>, new();
    }
}
