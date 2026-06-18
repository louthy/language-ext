using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    extension<A>(IteratorIO<A> self)
    {
        public static IteratorIO<A> operator |(IteratorIO<A> lhs, IteratorIO<A> rhs) =>
            +lhs.Choose(rhs);

        public static IteratorIO<A> operator |(IteratorIO<A> lhs, Pure<A> rhs) =>
            +lhs.Choose(IteratorIO.singleton(rhs.Value));
    }
    
    extension<A>(K<IteratorIO, A> self)
    {
        public static IteratorIO<A> operator |(K<IteratorIO, A> lhs, K<IteratorIO, A> rhs) =>
            +lhs.Choose(rhs);

        public static IteratorIO<A> operator |(K<IteratorIO, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(IteratorIO.singleton(rhs.Value));
    }
}
