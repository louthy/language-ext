//using System.Diagnostics.Contracts;

//namespace LanguageExt.TypeClasses
//{
//    [Typeclass]
//    public interface MonadPlus<MA, A> : Monad<MA, A>
//    {
//        /// <summary>
//        /// Associative binary operation
//        /// </summary>
//        [Pure]
//        MA Plus(MA a, MA b);

//        /// <summary>
//        /// Neutral element (None in Option for example)
//        /// </summary>
//        [Pure]
//        MA Zero();
//    }

//    [Typeclass]
//    public interface MonadPlus<Env, MA, A> : Monad<Env, MA, A>
//    {
//        /// <summary>
//        /// Associative binary operation
//        /// </summary>
//        [Pure]
//        MA Plus(MA a, MA b);

//        /// <summary>
//        /// Neutral element (None in Option for example)
//        /// </summary>
//        [Pure]
//        MA Zero();
//    }

//}
