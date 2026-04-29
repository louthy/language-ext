using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class RuleKExtensions
{
    extension<R1, F, A>(R1)
        where R1 : RuleK<R1, F, A>, new()
    {
        public static Fin<K<F, A>> ValidateK(
            K<F, A> value, Func<R1, K<F, A>, Error> Fail) =>
            R1.ValidateK(value, Fail);

        public static Fin<K<F, A>> ValidateK(K<F, A> value, Func<Error> Fail) =>
            R1.ValidateK(value, (_, _) => Fail());

        public static Fin<K<F, A>> ValidateK(K<F, A> value, Error Fail) =>
            R1.ValidateK(value, (_, _) => Fail);
    }
}
