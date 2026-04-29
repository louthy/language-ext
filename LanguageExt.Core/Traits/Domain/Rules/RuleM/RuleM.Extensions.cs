using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Traits.Domain;

public static partial class RuleMExtensions
{
    extension<SELF, M, A>(SELF)
        where SELF : RuleM<SELF, M, A>, new()
        where M : Monad<M>
    {
        public static FinT<M, A> ValidateM(A value, Func<SELF, A, K<M, Error>> Fail) =>
            SELF.ValidateM(value, Fail);

        public static FinT<M, A> ValidateM(A value, Func<K<M, Error>> Fail) =>
            SELF.ValidateM(value, (_, _) => Fail());

        public static FinT<M, A> ValidateM(A value, K<M, Error> Fail) =>
            SELF.ValidateM(value, (_, _) => Fail);

    }

}
