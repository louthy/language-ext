using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec.DSL;

static class DSL<E, S, T, M>
    where M : Monad<M>
    where S : TokenStream<S, T>
{
    public static ParsecT<E, S, T, M, Unit> eof
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ParsecTEOF<E, S, T, M>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> error<A>(
        ParseError<T, E> error) =>
        new ParsecTError<E, S, T, M, A>(error);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> label<A>(
        string name, 
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTLabel<E, S, T, M, A>(name, p.As());
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> @try<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTTry<E, S, T, M, A>(p.As());
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> lookAhead<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTLookAhead<E, S, T, M, A>(p.As());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, Unit> notFollowedBy<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTNotFollowedBy<E, S, T, M, A>(p.As());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, S> take(
        in Option<string> name, 
        int n) =>
        new ParsecTTake<E, S, T, M>(name, n);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> withRecovery<A>(
        Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> recovery, 
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTWithRecovery<E, S, T, M, A>(recovery, p.As());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, Either<ParseError<T, E>, A>> observing<A>(
        K<ParsecT<E, S, T, M>, A> p) =>
        new ParsecTObserving<E, S, T, M, A>(p.As());
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> token<A>(
        Func<T, Option<A>> test, 
        in Set<ErrorItem<T>> expected) =>
        new ParsecTToken<E, S, T, M, A>(test, expected);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, S> tokens(
        Func<S, S, bool> test, 
        in S chunk) =>
        new ParsecTTokens<E, S, T, M>(test, chunk);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, S> takeWhile(
        in Option<string> name, 
        Func<T, bool> test) =>
        new ParsecTTakeWhile<E, S, T, M>(name, test);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, S> takeWhile1(
        in Option<string> name, 
        Func<T, bool> test) =>
        new ParsecTTakeWhile1<E, S, T, M>(name, test);
    
    public static ParsecT<E, S, T, M, State<S, T, E>> parserState 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ParsecTParserState<E, S, T, M>.Default;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, Unit> updateParserState(
        Func<State<S, T, E>, State<S, T, E>> f) =>
        new ParsecTUpdateParserState<E, S, T, M>(f);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> lift<A>(
        Func<State<S, T, E>, Reply<E, S, T, A>> f) =>
        new ParsecTLift<E, S, T, M, A>(f);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> liftM<A>(K<M, A> ma) =>
        new ParsecTMTransLift<E, S, T, M, A>(ma);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> liftIO<A>(IO<A> ma) =>
        new ParsecTMTransLiftIO<E, S, T, M, A>(ma);
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, B> map<A, B>(
        ParsecT<E, S, T, M, A> p, 
        Func<A, B> f) =>
        new ParsecTMap<E, S, T, M, A, B>(p, f);    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, B> apply<A, B>(
        K<ParsecT<E, S, T, M>, Func<A, B>> ff, 
        K<ParsecT<E, S, T, M>, A> fa) =>
        new ParsecTApply<E, S, T, M, A, B>(ff.As(), fa.As());
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, B> bind<A, B>(
        K<ParsecT<E, S, T, M>, A> ma, 
        Func<A, K<ParsecT<E, S, T, M>, B>> f) =>
        new ParsecTBind<E, S, T, M, A, B>(ma.As(), f);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> choose<A>(
        K<ParsecT<E, S, T, M>, A> m, 
        K<ParsecT<E, S, T, M>, A> n) =>
        new ParsecTChoose<E, S, T, M, A>(m.As(), n.As());
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> choose<A>(
        K<ParsecT<E, S, T, M>, A> m, 
        Func<K<ParsecT<E, S, T, M>, A>> n) =>
        new ParsecTChooseLazy<E, S, T, M, A>(m.As(), n);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> @catch<A>(
        K<ParsecT<E, S, T, M>, A> fa,
        Func<ParseError<T, E>, bool> predicate, 
        Func<ParseError<T, E>, K<ParsecT<E, S, T, M>, A>> fail) =>
        new ParsecTCatch<E, S, T, M, A>(fa.As(), predicate, fail);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> fail<A>(string message) =>
        new ParsecTFail1<E, S, T, M, A>(message);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParsecT<E, S, T, M, A> fail<A>(E error) =>
        new ParsecTFail2<E, S, T, M, A>(error);    
}
