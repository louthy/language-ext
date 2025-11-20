using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Divisible<Supertype, Subtype> : 
        Cofunctor<Supertype, Subtype>, 
        Divisible<Supertype>
        where Subtype : Decidable<Subtype>
        where Supertype : Decidable<Supertype, Subtype>
    {
        static K<Supertype, A> Divisible<Supertype>.Divide<A, B, C>(
            Func<A, (B Left, C Right)> f, 
            K<Supertype, B> fb, 
            K<Supertype, C> fc) => 
            Supertype.CoTransform(Subtype.Divide(f, Supertype.Transform(fb), Supertype.Transform(fc)));

        static K<Supertype, A> Divisible<Supertype>.Conquer<A>() => 
            Supertype.CoTransform(Subtype.Conquer<A>());
    }
}
