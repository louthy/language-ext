using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public interface M<A> : AP<A>
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="a">The bound monad value</param>
        /// <returns>Monad of A</returns>
        M<A> Return(A a);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        M<B> Bind<B>(M<A> ma, Func<A, M<B>> f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        M<A> Fail(string err = "");
    }
}
