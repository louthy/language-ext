using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTPutState<E, S, T, M>(State<S, T, E> State) : 
    ParsecT<E, S, T, M, Unit>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, Unit, B> _,
        ConsumedErr<E, S, T, M, B> __,
        EmptyOK<E, S, T, M, Unit, B> eok,
        EmptyErr<E, S, T, M, B> ___) =>
        eok(unit, State, Hints<T>.Empty);
}

record ParsecTModifyState<E, S, T, M>(Func<State<S, T, E>, State<S, T, E>> Update) : 
    ParsecT<E, S, T, M, Unit>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public override K<M, B> Run<B>(
        State<S, T, E> s,
        ConsumedOK<E, S, T, M, Unit, B> _,
        ConsumedErr<E, S, T, M, B> __,
        EmptyOK<E, S, T, M, Unit, B> eok,
        EmptyErr<E, S, T, M, B> ___) =>
        eok(unit, Update(s), Hints<T>.Empty);
}
