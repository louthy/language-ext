using System;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class RuleExtensions
{
    extension<R1, A>(R1)
        where R1 : Rule<R1, A>, new()
    {
        public static Fin<A> Validate(A value, Func<R1, A, Error> Fail) =>
            R1.Validate(value, Fail);

        public static Fin<A> Validate(A value, Func<A, Error> Fail) =>
            R1.Validate(value, (_, a) => Fail(a));

        public static Fin<A> Validate(A value, Func<Error> Fail) =>
            R1.Validate(value, (_, _) => Fail());

        public static Fin<A> Validate(A value, Error Fail) =>
            R1.Validate(value, (_, _) => Fail);
    }
}
