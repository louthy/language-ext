using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RuleIOExtensions
{
    extension<R, M, A>(RuleK<R, M, A>)
        where R : RuleK<R, M, A>, new()
        where M : Monad<M>
    {
        public static FinT<M, Unit> Validate(A value, Func<R, A, K<M, Error>> Fail) =>
            R.Validate(value, Fail);

        public static FinT<M, Unit> Validate(A value, Func<R, A, Error> Fail) =>
            R.Validate(value, (r, a) => M.Pure(Fail(r, a)));

        public static FinT<M, Unit> Validate(A value, Func<Error> Fail) =>
            R.Validate(value, (_, _) => M.Pure(Fail()));

        public static FinT<M, Unit> Validate(A value, Error Fail) =>
            R.Validate(value, (_, _) => M.Pure(Fail));
    }
}
