using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryTExtensions
{
    extension<M, A>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        public static TryT<M, A> operator +(K<TryT<M>, A> lhs, K<TryT<M>, A> rhs) =>
            +lhs.Combine(rhs);

        public static TryT<M, A> operator +(K<TryT<M>, A> lhs, Pure<A> rhs) =>
            +lhs.Combine(TryT.Succ<M, A>(rhs.Value));
        
        public static TryT<M, A> operator +(K<TryT<M>, A> lhs, Fail<Error> rhs) =>
            +lhs.Combine(TryT.Fail<M, A>(rhs.Value));

        public static TryT<M, A> operator +(K<TryT<M>, A> lhs, Fail<Exception> rhs) =>
            +lhs.Combine(TryT.Fail<M, A>(rhs.Value));

        public static TryT<M, A> operator +(K<TryT<M>, A> lhs, Error rhs) =>
            +lhs.Combine(TryT.Fail<M, A>(rhs));

        public static TryT<M, A> operator +(K<TryT<M>, A> lhs, Exception rhs) =>
            +lhs.Combine(TryT.Fail<M, A>(rhs));
    }
}
