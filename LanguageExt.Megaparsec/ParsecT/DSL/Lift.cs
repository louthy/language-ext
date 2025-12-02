using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

record ParsecTLift<E, S, T, M, A>(Func<State<S, T, E>, Reply<E, S, T, A>> F) : 
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
        F(s) switch
        {
            (var s1, true, var result) =>
                result switch
                {
                    Result<T, E, A>.OK(var hs, var x) => cok(x, s1, hs),
                    Result<T, E, A>.Error(var e)      => cerr(e, s1),
                    _                                 => throw new NotSupportedException()
                },
            (var s1, false, var result) =>
                result switch
                {
                    Result<T, E, A>.OK(var hs, var x) => eok(x, s1, hs),
                    Result<T, E, A>.Error(var e)      => eerr(e, s1),
                    _                                 => throw new NotSupportedException()
                }
        };
}

record ParsecTLift2<E, S, T, M, A>(Func<State<S, T, E>, K<M, Reply<E, S, T, A>>> F) : 
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
        F(s) >>> (f => f switch
        {
            (var s1, true, var result) =>
                result switch
                {
                    Result<T, E, A>.OK(var hs, var x) => cok(x, s1, hs),
                    Result<T, E, A>.Error(var e)      => cerr(e, s1),
                    _                                 => throw new NotSupportedException()
                },
            (var s1, false, var result) =>
                result switch
                {
                    Result<T, E, A>.OK(var hs, var x) => eok(x, s1, hs),
                    Result<T, E, A>.Error(var e)      => eerr(e, s1),
                    _                                 => throw new NotSupportedException()
                }
        });
}
