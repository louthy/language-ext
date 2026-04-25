using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RuleExtensions
{
    extension<R, A>(Rule<R, A>)
        where R : Rule<R, A>, new()
    {
        public static Fin<Unit> Validate(A value, Func<R, A, Error> Fail) =>
            R.Validate(value, Fail);

        public static Fin<Unit> Validate(A value, Func<Error> Fail) =>
            R.Validate(value, (_, _) => Fail());

        public static Fin<Unit> Validate(A value, Error Fail) =>
            R.Validate(value, (_, _) => Fail);
    }

}
