using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryExtensions
{
    extension<A>(K<Try, A> self)
    {
        public static Try<A> operator +(K<Try, A> lhs, K<Try, A> rhs) =>
            +lhs.Combine(rhs);

        public static Try<A> operator +(K<Try, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(rhs.ToTry());

        public static Try<A> operator +(K<Try, A> lhs, Fail<Error> rhs) =>
            +lhs.Combine(Try.Fail<A>(rhs.Value));

        public static Try<A> operator +(K<Try, A> lhs, Fail<Exception> rhs) =>
            +lhs.Combine(Try.Fail<A>(rhs.Value));

        public static Try<A> operator +(K<Try, A> lhs, Error rhs) =>
            +lhs.Combine(Try.Fail<A>(rhs));

        public static Try<A> operator +(K<Try, A> lhs, Exception rhs) =>
            +lhs.Combine(Try.Fail<A>(rhs));        
    }
}
