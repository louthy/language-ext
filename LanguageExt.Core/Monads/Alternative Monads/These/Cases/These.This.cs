using System;

namespace LanguageExt;

public partial class These
{
    public sealed record This<A, B>(A Value) : These<A, B>
    {
        public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Both) =>
            This(Value);
    
        public override (A, B) ToTuple(A x, B y) =>
            (Value, y);

        public override These<A, C> Map<C>(Func<B, C> f) =>
            new This<A, C>(Value);

        public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
            new This<C, D>(This(Value));   
    }    
}
