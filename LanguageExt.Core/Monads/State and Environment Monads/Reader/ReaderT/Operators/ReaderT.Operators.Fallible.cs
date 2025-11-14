using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, E, M, A>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>, Fallible<E, M>
    {
        public static ReaderT<Env, M, A> operator |(K<ReaderT<Env, M>, A> lhs, CatchM<E, M, A> rhs) =>
            new(env => lhs.As().runReader(env) | rhs);

        public static ReaderT<Env, M, A> operator |(K<ReaderT<Env, M>, A> lhs, Fail<E> rhs) =>
            new(env => lhs.As().runReader(env) | rhs);
    }
}
