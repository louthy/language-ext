using System;

namespace LanguageExt;

public partial class These
{
    public sealed record Both<A, B>(A First, B Second) : These<A, B>
    {
        public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Both) =>
            Both(First, Second);   
    
        public override (A, B) ToTuple(A x, B y) =>
            (First, Second);    

        public override These<A, C> Map<C>(Func<B, C> f) =>
            new Both<A, C>(First, f(Second));
    
        public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
            new Both<C, D>(This(First), That(Second));
    }
}
