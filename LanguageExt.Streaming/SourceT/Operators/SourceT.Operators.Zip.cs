using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    extension<M, A, B>(K<SourceT<M>, A>)
        where M : MonadIO<M>, Fallible<M>
    {
        public static SourceT<M, (A First, B Second)> operator &(K<SourceT<M>, A> lhs, K<SourceT<M>, B> rhs) =>
            +lhs.Zip(rhs);
    }
    
    extension<M, A, B, C>(K<SourceT<M>, (A First, B Second)>)
        where M : MonadIO<M>, Fallible<M>
    {
        public static SourceT<M, (A First, B Second, C Third)> operator &(K<SourceT<M>, (A First, B Second)> lhs, K<SourceT<M>, C> rhs) =>
            +lhs.Zip(rhs).Map(s => (s.First.First, s.First.Second, s.Second));
    }    
    
    extension<M, A, B, C, D>(K<SourceT<M>, (A First, B Second)>)
        where M : MonadIO<M>, Fallible<M>
    {
        public static SourceT<M, (A First, B Second, C Third, D Fourth)> operator &(K<SourceT<M>, (A First, B Second)> lhs, K<SourceT<M>, (C First, D Second)> rhs) =>
            +lhs.Zip(rhs).Map(s => (s.First.First, s.First.Second, s.Second.First, s.Second.Second));
    }    
    
    extension<M, A, B, C, D>(K<SourceT<M>, (A First, B Second, C Third)>)
        where M : MonadIO<M>, Fallible<M>
    {
        public static SourceT<M, (A First, B Second, C Third, D Fourth)> operator &(K<SourceT<M>, (A First, B Second, C Third)> lhs, K<SourceT<M>, D> rhs) =>
            +lhs.Zip(rhs).Map(s => (s.First.First, s.First.Second, s.First.Third, s.Second));
    }
}
