using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derive the class of contravariant functors.
    /// 
    /// Whereas one can think of a `Functor` as containing or producing values, a contravariant functor is a functor that
    /// can be thought of as consuming values.
    /// 
    /// Contravariant functors are referred to colloquially as Cofunctor, even though the dual of a `Functor` is just
    /// a `Functor`. 
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface Cofunctor<Supertype, Subtype> : 
        Cofunctor<Supertype>, 
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Subtype : Cofunctor<Subtype>
        where Supertype : Cofunctor<Supertype, Subtype>
    {
        static K<Supertype, A> Cofunctor<Supertype>.Contramap<A, B>(K<Supertype, B> fb, Func<A, B> f) =>
            Supertype.CoTransform(Subtype.Contramap(Supertype.Transform(fb), f));
    }
}
