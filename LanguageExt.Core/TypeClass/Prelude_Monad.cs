using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public static partial class Prelude
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="a">The bound monad value</param>
        /// <returns>Monad of A</returns>
        public static M Return<M, A>(A a) where M : struct, M<A> =>
            (M)default(M).Return(a);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        public static M<B> bind<M, A, B>(M<A> ma, Func<A, M<B>> f) where M : struct, M<A> =>
            default(M).Bind(ma, f);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        public static M fail<M, A>(string err = "") where M : struct, M<A> =>
            (M)default(M).Fail(err);
    }
}
