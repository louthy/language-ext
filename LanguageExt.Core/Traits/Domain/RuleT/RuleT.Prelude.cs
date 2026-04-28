using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public partial class ruleTFor<T, M, A>
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        public sealed class All<R1, R2>
            : RuleT.All<R1, R2, T, M, A>, RuleT<All<R1, R2>, T, M, A>
            where R1 : RuleT<R1, T, M, A>, new()
            where R2 : RuleT<R2, T, M, A>, new();

        public sealed class Any<R1, R2>
            : RuleT.Any<R1, R2, T, M, A>, RuleT<Any<R1, R2>, T, M, A>
            where R1 : RuleT<R1, T, M, A>, new()
            where R2 : RuleT<R2, T, M, A>, new();

        public sealed class Not<R>
            : RuleT.Not<R, T, M, A>, RuleT<Not<R>, T, M, A>
            where R : RuleT<R, T, M, A>, new();

        public sealed class Lift<R>
            : RuleT.Lift<R, T, M, A>, RuleT<Lift<R>, T, M, A>
            where R : RuleK<R, M, A>, new();
    }
}
