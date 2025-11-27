using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTAsk<E, S, T, M> :
    ParsecT<E, S, T, M, State<S, T, E>>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public static readonly ParsecT<E, S, T, M, State<S, T, E>> Default = new ParsecTAsk<E, S, T, M>();
    
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, State<S, T, E>, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, State<S, T, E>, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        eok(s, s, Hints<T>.Empty);
}

record ParsecTAsks<E, S, T, M, A>(Func<State<S, T, E>, A> F) :
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        eok(F(s), s, Hints<T>.Empty);
}

record ParsecTLocal<E, S, T, M, A>(Func<State<S, T, E>, State<S, T, E>> F, ParsecT<E, S, T, M, A> P) :
    ParsecT<E, S, T, M, A>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, A, B> cok,
        ConsumedErr<E, S, T, M, B> cerr,
        EmptyOK<E, S, T, M, A, B> eok,
        EmptyErr<E, S, T, M, B> eerr) =>
        P.Run(F(s), cok, cerr, eok, eerr);
}
