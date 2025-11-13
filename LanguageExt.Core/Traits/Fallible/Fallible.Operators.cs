using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FallibleExtensions
{
    extension<E, F, A>(K<F, A> _)
        where F : Fallible<E, F>
    {
        /// <summary>
        /// Catch operator.  Catch an error if the predicate in the structure matches. 
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        public static K<F, A> operator |(K<F, A> lhs, CatchM<E, F, A> rhs) =>
            lhs.Catch(rhs);
        
        /// <summary>
        /// Catch operator.  Catch an error if the predicate in the structure matches. 
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        public static K<F, A> operator |(K<F, A> lhs, Fail<E> rhs) =>
            lhs.Catch(rhs);
    }
    
    extension<F, A>(K<F, A> _)
        where F : Fallible<F>
    {
        /// <summary>
        /// Catch operator.  Catch an error if the predicate in the structure matches. 
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        public static K<F, A> operator |(K<F, A> lhs, Error rhs) =>
            lhs.Catch(rhs);
    }    
}
