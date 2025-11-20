using System;

namespace LanguageExt;

public abstract partial record These<A, B>
{
    public sealed record This(A Value) : These<A, B>
    {
        public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Both) =>
            This(Value);
    
        public override (A, B) ToTuple(A x, B y) =>
            (Value, y);

        public override These<A, C> Map<C>(Func<B, C> f) =>
            new These<A, C>.This(Value);

        public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
            new These<C, D>.This(This(Value));
    }    
}
