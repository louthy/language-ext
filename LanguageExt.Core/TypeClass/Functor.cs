using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    public interface Functor<A> : Iterable<A>
    {
        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        Functor<B> Map<B>(Functor<A> fa, Func<A, B> fab);
    }
}
