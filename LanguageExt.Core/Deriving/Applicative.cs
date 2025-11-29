using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived applicative functor implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface Applicative<Supertype, Subtype> :
        Applicative<Supertype>,
        Functor<Supertype, Subtype>
        where Subtype : Applicative<Subtype>
        where Supertype : Applicative<Supertype, Subtype>
    {
        static K<Supertype, A> Applicative<Supertype>.Pure<A>(A value) => 
            Supertype.CoTransform(Subtype.Pure(value));

        static K<Supertype, B> Applicative<Supertype>.Action<A, B>(K<Supertype, A> ma, K<Supertype, B> mb) => 
            Supertype.CoTransform(Subtype.Action(Supertype.Transform(ma), Supertype.Transform(mb)));

        static K<Supertype, B> Applicative<Supertype>.Apply<A, B>(K<Supertype, Func<A, B>> mf, K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.Apply(Supertype.Transform(mf), Supertype.Transform(ma)));

        static K<Supertype, B> Applicative<Supertype>.Apply<A, B>(K<Supertype, Func<A, B>> mf, Memo<Supertype, A> ma) => 
            Supertype.CoTransform(Subtype.Apply(Supertype.Transform(mf), Memo.transform<Supertype, Subtype, A>(ma)));
    }
}
