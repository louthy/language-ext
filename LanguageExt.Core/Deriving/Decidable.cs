using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derive a `Decidable` contravariant functor that is the contravariant analogue of `Alternative`.
    /// 
    /// Noting the superclass constraint that `f` must also be `Divisible`, a `Decidable` functor has the ability to
    /// "fan out" input, under the intuition that contravariant functors consume input.
    /// </summary>
    /// <typeparam name="F">Self referring type</typeparam>
    public interface Decidable<Supertype, Subtype> : 
        Divisible<Supertype, Subtype>, 
        Decidable<Supertype>
        where Subtype : Decidable<Subtype>
        where Supertype : Decidable<Supertype, Subtype>
    {
        static K<Supertype, A> Decidable<Supertype>.Lose<A>(Func<A, Void> f) => 
            Supertype.CoTransform(Subtype.Lose(f));

        static K<Supertype, A> Decidable<Supertype>.Route<A, B, C>(
            Func<A, Either<B, C>> f, 
            K<Supertype, B> fb, 
            K<Supertype, C> fc) => 
            Supertype.CoTransform(Subtype.Route(f, Supertype.Transform(fb), Supertype.Transform(fc)));
    }
}
