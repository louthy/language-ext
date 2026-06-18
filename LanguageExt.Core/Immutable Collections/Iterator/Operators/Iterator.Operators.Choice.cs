using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorExtensions
{
    extension<A>(Iterator<A> self)
    {
        public static Iterator<A> operator |(Iterator<A> lhs, Iterator<A> rhs) =>
            +lhs.Choose(rhs);

        public static Iterator<A> operator |(Iterator<A> lhs, Pure<A> rhs) =>
            +lhs.Choose(Iterator.singleton(rhs.Value));
    }
    
    extension<A>(K<Iterator, A> self)
    {
        public static Iterator<A> operator |(K<Iterator, A> lhs, K<Iterator, A> rhs) =>
            +lhs.Choose(rhs);

        public static Iterator<A> operator |(K<Iterator, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(Iterator.singleton(rhs.Value));
    }
}
