using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    /// <summary>
    /// Derived `Readable` implementation
    /// </summary>
    /// <typeparam name="Supertype">Super-type wrapper around the subtype</typeparam>
    /// <typeparam name="Env">Reader environment</typeparam>
    /// <typeparam name="Subtype">The subtype that the supertype type 'wraps'</typeparam>
    public interface Readable<Supertype, Env, Subtype> : 
        Readable<Supertype, Env>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : Readable<Supertype, Env, Subtype>, Readable<Supertype, Env>
        where Subtype : Readable<Subtype, Env>
    {
        static K<Supertype, Env> Readable<Supertype, Env>.Ask => 
            Supertype.CoTransform(Subtype.Ask);

        static K<Supertype, A> Readable<Supertype, Env>.Asks<A>(Func<Env, A> f) => 
            Supertype.CoTransform(Subtype.Asks(f));

        static K<Supertype, A> Readable<Supertype, Env>.Local<A>(Func<Env, Env> f, K<Supertype, A> ma) => 
            Supertype.CoTransform(Supertype.Transform(ma).Local(f));
    }
}
