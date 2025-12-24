using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    extension<W, E, M, A>(K<WriterT<W, M>, A> self)
        where M : Monad<M>, Fallible<E, M>
        where W : Monoid<W>
    {
        public static WriterT<W, M, A> operator |(K<WriterT<W, M>, A> lhs, CatchM<E, M, A> rhs) =>
            new(output => lhs.As().runWriter(output) | rhs.Map(a => (a, env: output)));

        public static WriterT<W, M, A> operator |(K<WriterT<W, M>, A> lhs, Fail<E> rhs) =>
            new(output => lhs.As().runWriter(output) | rhs);
    }
}
