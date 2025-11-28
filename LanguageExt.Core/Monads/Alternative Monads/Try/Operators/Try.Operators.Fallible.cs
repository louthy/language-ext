using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryExtensions
{
    extension<A>(K<Try, A> self)
    {
        public static Try<A> operator |(K<Try, A> lhs, CatchM<Error, Try, A> rhs) =>
            +lhs.Catch(rhs);

        public static Try<A> operator |(K<Try, A> lhs, Fail<Error> rhs) =>
            +lhs.Catch(rhs);

        public static Try<A> operator |(K<Try, A> lhs, Fail<Exception> rhs) =>
            +lhs.Choose(Try.Fail<A>(rhs.Value));

        public static Try<A> operator |(K<Try, A> lhs, Error rhs) =>
            +lhs.Catch(rhs);

        public static Try<A> operator |(K<Try, A> lhs, Exception rhs) =>
            +lhs.Catch(rhs);
    }
}
