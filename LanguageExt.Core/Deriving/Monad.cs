using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived monad implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface Monad<Supertype, Subtype> :
        MonadIO<Supertype, Subtype>,
        Applicative<Supertype, Subtype>,
        Monad<Supertype>
        where Subtype : Monad<Subtype>
        where Supertype : Monad<Supertype, Subtype>
    {
        /// <summary>
        /// Monad bind operation.  Chains two operations together in sequence.
        /// </summary>
        /// <param name="ma">First monad to run</param>
        /// <param name="f">Bind function that yields the second monad to run</param>
        /// <typeparam name="A">Input bound value type</typeparam>
        /// <typeparam name="B">Output bound value type</typeparam>
        /// <returns>Composed chained monad operation</returns>
        static K<Supertype, B> Monad<Supertype>.Bind<A, B>(K<Supertype, A> ma, Func<A, K<Supertype, B>> f) => 
            Supertype.CoTransform(Supertype.Transform(ma).Bind(x => Supertype.Transform(f(x))));
    }
}
