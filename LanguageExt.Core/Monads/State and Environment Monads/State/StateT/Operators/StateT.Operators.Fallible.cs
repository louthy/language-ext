using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<S, E, M, A>(K<StateT<S, M>, A> self)
        where M : Monad<M>, Fallible<E, M>
    {
        public static StateT<S, M, A> operator |(K<StateT<S, M>, A> lhs, CatchM<E, M, A> rhs) =>
            new(s => lhs.As().runState(s) | rhs.Map(a => (a, env: s)));

        public static StateT<S, M, A> operator |(K<StateT<S, M>, A> lhs, Fail<E> rhs) =>
            new(s => lhs.As().runState(s) | rhs);
    }
}
