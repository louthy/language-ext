using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SeqExtensions
{
    extension<A>(K<Lst, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Lst<A> operator +(K<Lst, A> ma) =>
            (Lst<A>)ma;
        
        public static Lst<A> operator >> (K<Lst, A> ma, Lower lower) =>
            (Lst<A>)ma;
    }
}
