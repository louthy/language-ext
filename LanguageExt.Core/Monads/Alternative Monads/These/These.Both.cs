using System;

namespace LanguageExt;

public abstract partial record These<A, B>
{
    public sealed record Both(A First, B Second) : These<A, B>
    {
        public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Both) =>
            Both(First, Second);   
    
        public override (A, B) ToTuple(A x, B y) =>
            (First, Second);    

        public override These<A, C> Map<C>(Func<B, C> f) =>
            new These<A, C>.Both(First, f(Second));

        public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
            new These<C, D>.Both(This(First), That(Second));
    }
}
