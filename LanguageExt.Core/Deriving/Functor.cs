using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived functor implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface Functor<Supertype, Subtype> : 
        Functor<Supertype>, 
        Natural<Supertype, Subtype>,
        CoNatural<Supertype, Subtype>
        where Subtype : Functor<Subtype>
        where Supertype : Functor<Supertype, Subtype>
    {
        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <param name="f">Mapping function</param>
        /// <param name="ma">Functor structure to map</param>
        /// <typeparam name="A">Input bound value type</typeparam>
        /// <typeparam name="B">Output bound value type</typeparam>
        /// <returns>Mapped functor</returns>
        static K<Supertype, B> Functor<Supertype>.Map<A, B>(Func<A, B> f, K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.Map(f, Supertype.Transform(ma)));
    }
}
