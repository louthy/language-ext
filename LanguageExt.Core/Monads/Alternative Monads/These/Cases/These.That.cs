using System;

namespace LanguageExt;

public partial class These
{
    public sealed record That<A, B>(B Value) : These<A, B>
    {
        public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Both) =>
            That(Value);
    
        public override (A, B) ToTuple(A x, B y) =>
            (x, Value);

        public override These<A, C> Map<C>(Func<B, C> f) =>
            new That<A, C>(f(Value));
    
        public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
            new That<C, D>(That(Value));  
    }
}
