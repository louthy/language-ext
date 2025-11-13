using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A, B>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static Eff<RT, B> operator >> (K<Eff<RT>, A> ma, Func<A, K<Eff<RT>, B>> f) =>
            ma.Bind(f).As();
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static Eff<RT, B> operator >> (K<Eff<RT>, A> lhs, K<Eff<RT>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<RT, A>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static Eff<RT, A> operator >> (K<Eff<RT>, A> lhs, K<Eff<RT>, Unit> rhs) =>
            lhs >> (x => (_ => x) * rhs);
    }
}
