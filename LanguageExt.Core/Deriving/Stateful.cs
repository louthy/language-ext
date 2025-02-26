using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Stateful<Supertype, Subtype, S> :
        Stateful<Supertype, S>,
        Natural<Supertype, Subtype>,
        CoNatural<Supertype, Subtype>
        where Supertype : Stateful<Supertype, Subtype, S>, Stateful<Supertype, S>
        where Subtype : Stateful<Subtype, S>
    {
        static K<Supertype, Unit> Stateful<Supertype, S>.Put(S value) =>
            Supertype.CoTransform(Subtype.Put(value));

        static K<Supertype, Unit> Stateful<Supertype, S>.Modify(Func<S, S> modify) =>
            Supertype.CoTransform(Subtype.Modify(modify));

        static K<Supertype, A> Stateful<Supertype, S>.Gets<A>(Func<S, A> f) =>
            Supertype.CoTransform(Subtype.Gets(f));

        static K<Supertype, S> Stateful<Supertype, S>.Get => 
            Supertype.CoTransform(Subtype.Get);
    }
}
