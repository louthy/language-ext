using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A, B>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Binding function</param>
        /// <returns>Mapped monad</returns>
        public static ChronicleT<Ch, M, B> operator >> (K<ChronicleT<Ch, M>, A> ma, Func<A, K<ChronicleT<Ch, M>, B>> f) =>
            ma.Bind(f).As();
        
        /// <summary>
        /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
        /// as the semicolon) in C#.
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the second action</returns>
        public static ChronicleT<Ch, M, B> operator >> (K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, B> rhs) =>
            lhs >> (_ => rhs);
    }
    
    extension<Ch, M, A>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Sequentially compose two actions.  The second action is a unit-returning action, so the result of the
        /// first action is propagated. 
        /// </summary>
        /// <param name="lhs">First action to run</param>
        /// <param name="rhs">Second action to run</param>
        /// <returns>Result of the first action</returns>
        public static ChronicleT<Ch, M, A> operator >> (K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, Unit> rhs) =>
            lhs >> (x => rhs * (_ => x));
    }
}
