using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, E, M, A>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>, Fallible<E, M>
        where W : Monoid<W>
    {
        public static RWST<R, W, S, M, A> operator |(K<RWST<R, W, S, M>, A> lhs, CatchM<E, M, A> rhs) =>
            new(env => lhs.As().runRWST(env) | rhs.Map(a => (a, env.Output, env.State)));

        public static RWST<R, W, S, M, A> operator |(K<RWST<R, W, S, M>, A> lhs, Fail<E> rhs) =>
            new(env => lhs.As().runRWST(env) | rhs);
    }
}
