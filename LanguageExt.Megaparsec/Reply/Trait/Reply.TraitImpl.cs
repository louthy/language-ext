using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public class Reply<E, S, T> : Functor<Reply<E, S, T>>
{
    static K<Reply<E, S, T>, B> Functor<Reply<E, S, T>>.Map<A, B>(Func<A, B> f, K<Reply<E, S, T>, A> ma) =>
        ma switch
        {
            Reply<E, S, T, A> r => new Reply<E, S, T, B>(r.NewState, r.Consumed, f * r.Result),
            _                   => throw new Exception("Impossible")
        };
}
