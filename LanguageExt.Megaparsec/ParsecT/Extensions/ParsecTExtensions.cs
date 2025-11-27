using LanguageExt.Megaparsec.DSL;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static class ParsecTExtensions
{
    extension<E, S, T, M, A>(K<ParsecT<E, S, T, M>, A> self) 
        where S : TokenStream<S, T> 
        where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public ParsecT<E, S, T, M, A> As() =>
            (ParsecT<E, S, T, M, A>)self;

        /// <summary>
        /// Run the parser
        /// </summary>
        /// <param name="initialState">Starting state</param>
        /// <returns>Result of the parse</returns>
        public K<M, Reply<E, S, T, A>> Run(State<S, T, E> initialState)
        {
            return self.As().Run(initialState, cok, cerr, eok, eerr);
        
            K<M, Reply<E, S, T, A>> cok(A x, State<S, T, E> s1, Hints<T> hs) =>
                M.Pure(Reply.ConsumedOK(x, s1, hs));
        
            K<M, Reply<E, S, T, A>> cerr(ParseError<T, E> e, State<S, T, E> s1) =>
                M.Pure(Reply.ConsumedError<E, S, T, A>(e, s1));        
        
            K<M, Reply<E, S, T, A>> eok(A x, State<S, T, E> s1, Hints<T> hs) =>
                M.Pure(Reply.EmptyOK(x, s1, hs));
        
            K<M, Reply<E, S, T, A>> eerr(ParseError<T, E> e, State<S, T, E> s1) =>
                M.Pure(Reply.EmptyError<E, S, T, A>(e, s1));        
        }

        /// <summary>
        /// Invoke this parser and then feed the lifted reply to the `f` function provided.
        /// Then rewrap the resulting lifted reply into a new parser. 
        /// </summary>
        /// <param name="f">Reply mapping function</param>
        /// <typeparam name="B">New parser result type</typeparam>
        /// <returns>Parser</returns>
        public ParsecT<E, S, T, M, B> Hoist<B>(Func<K<M, Reply<E, S, T, A>>, K<M, Reply<E, S, T, B>>> f) =>
            DSL<E, S, T, M>.lift<B>(s => f(self.Run(s)));        
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static ParsecT<E, S, T, M, A> operator +(K<ParsecT<E, S, T, M>, A> ma) =>
            ma.As();

        /// <summary>
        /// Downcast operator
        /// </summary>
        public static ParsecT<E, S, T, M, A> operator >>(K<ParsecT<E, S, T, M>, A> lhs, Lower _) =>
            lhs.As();
    }

}
