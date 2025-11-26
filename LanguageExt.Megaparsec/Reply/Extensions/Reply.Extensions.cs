using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static class ReplyExtensions
{
    extension<E, S, T, A>(K<Reply<E, S, T>, A> self)
    {
        public Reply<E, S, T, A> As() =>
            (Reply<E, S, T, A>)self;

        public static Reply<E, S, T, A> operator +(K<Reply<E, S, T>, A> ma) =>
            ma.As();
    }
}
