using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public class Reply<E, S> : Functor<Reply<E, S>>
{
    static K<Reply<E, S>, B> Functor<Reply<E, S>>.Map<A, B>(Func<A, B> f, K<Reply<E, S>, A> ma) =>
        ma switch
        {
            Reply<E, S, A> r => new Reply<E, S, B>(r.NewState, r.Consumed, f * r.Result),
            _                => throw new Exception("Impossible")
        };
}
