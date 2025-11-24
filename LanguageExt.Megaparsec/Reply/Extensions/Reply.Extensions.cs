using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static class ReplyExtensions
{
    extension<E, S, A>(K<Reply<E, S>, A> self)
    {
        public Reply<E, S, A> As() =>
            (Reply<E, S, A>)self;

        public static Reply<E, S, A> operator +(K<Reply<E, S>, A> ma) =>
            ma.As();
    }
}
