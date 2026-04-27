using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

public static partial class RuleMExtensions
{
    extension<SELF, M, A>(SELF)
        where SELF : RuleM<SELF, M, A>, new()
        where M : Monad<M>
    {
        public static FinT<M, A> Validate(A value, Func<SELF, A, K<M, Error>> Fail) =>
            SELF.Validate(value, Fail);

        public static FinT<M, A> Validate(A value, Func<K<M, Error>> Fail) =>
            SELF.Validate(value, (_, _) => Fail());

        public static FinT<M, A> Validate(A value, K<M, Error> Fail) =>
            SELF.Validate(value, (_, _) => Fail);

    }

}
