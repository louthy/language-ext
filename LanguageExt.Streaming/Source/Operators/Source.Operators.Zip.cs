using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<A, B>(K<Source, A>)
    {
        public static Source<(A First, B Second)> operator &(K<Source, A> lhs, K<Source, B> rhs) =>
            +lhs.Zip(rhs);
    }
    
    extension<A, B, C>(K<Source, (A First, B Second)>)
    {
        public static Source<(A First, B Second, C Third)> operator &(K<Source, (A First, B Second)> lhs, K<Source, C> rhs) =>
            +lhs.Zip(rhs).Map(s => (s.First.First, s.First.Second, s.Second));
    }    
    
    extension<A, B, C, D>(K<Source, (A First, B Second)>)
    {
        public static Source<(A First, B Second, C Third, D Fourth)> operator &(K<Source, (A First, B Second)> lhs, K<Source, (C First, D Second)> rhs) =>
            +lhs.Zip(rhs).Map(s => (s.First.First, s.First.Second, s.Second.First, s.Second.Second));
    }    
    
    extension<A, B, C, D>(K<Source, (A First, B Second, C Third)>)
    {
        public static Source<(A First, B Second, C Third, D Fourth)> operator &(K<Source, (A First, B Second, C Third)> lhs, K<Source, D> rhs) =>
            +lhs.Zip(rhs).Map(s => (s.First.First, s.First.Second, s.First.Third, s.Second));
    }
}

