using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static class ParsecTExtensions
{
    extension<E, S, T, M, A>(K<ParsecT<E, S, T, M>, A> self) where S : TokenStream<S, T> where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public ParsecT<E, S, T, M, A> As() =>
            (ParsecT<E, S, T, M, A>)self;

        /// <summary>
        /// Downcast operator
        /// </summary>
        public static ParsecT<E, S, T, M, A> operator +(K<ParsecT<E, S, T, M>, A> ma) =>
            ma.As();
    }
}
