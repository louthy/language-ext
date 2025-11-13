using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A, B>(K<IO, A> ma)
    {
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static IO<B> operator >>> (K<IO, A> lhs, K<IO, B> rhs) =>
            lhs.Action(rhs).As();
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static IO<B> operator >> (K<IO, A> lhs, K<IO, B> rhs) =>
            lhs.As().Bind(_ => rhs);
    }
    
    extension<A>(K<IO, A> ma)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static IO<A> operator >> (K<IO, A> lhs, K<IO, Unit> rhs) =>
            lhs.As().Bind(x => rhs.Map(_ => x));
    }
}
