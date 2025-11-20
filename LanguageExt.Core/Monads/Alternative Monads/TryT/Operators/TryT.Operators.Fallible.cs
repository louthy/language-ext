using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryTExtensions
{
    extension<M, A>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, CatchM<Error, TryT<M>, A> rhs) =>
            +lhs.Catch(rhs);

        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, Fail<Error> rhs) =>
            +lhs.Catch(rhs);

        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, Fail<Exception> rhs) =>
            +lhs.Catch(rhs.Value);

        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, Error rhs) =>
            +lhs.Catch(rhs);

        public static TryT<M, A> operator |(K<TryT<M>, A> lhs, Exception rhs) =>
            +lhs.Catch(rhs);
    }
}
