using System;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

public static partial class RuleTExtensions
{
    extension<SELF, T, M, A>(SELF)
        where SELF : RuleT<SELF, T, M, A>, new()
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        public static FinT<T, A> Validate(K<M, A> value, Func<SELF, A, K<T, Error>> Fail) =>
            SELF.Validate(value, Fail);

        public static FinT<T, A> Validate(K<M, A> value, Func<K<T, Error>> Fail) =>
            SELF.Validate(value, (_, _) => Fail());

        public static FinT<T, A> Validate(K<M, A> value, K<T, Error> Fail) =>
            SELF.Validate(value, (_, _) => Fail);

    }

}
