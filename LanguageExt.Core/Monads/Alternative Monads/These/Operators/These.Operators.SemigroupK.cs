using LanguageExt.Traits;
namespace LanguageExt;

public static partial class TheseExtensions
{
    extension<A, B>(K<These<A>, B> _) 
        where A : Semigroup<A>
        where B : Semigroup<B>
    {
        /// <summary>
        /// Semigroup combine operator: an associative binary operation.
        /// </summary>
        /// <param name="lhs">Left-hand side operand</param>
        /// <param name="rhs">Right-hand side operand</param>
        /// <returns></returns>
        public static K<These<A>, B> operator +(K<These<A>, B> lhs, K<These<A>, B> rhs) =>
            lhs.Combine(rhs);
    }
}
