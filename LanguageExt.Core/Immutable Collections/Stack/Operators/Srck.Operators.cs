using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StckExtensions
{
    extension<A>(K<Stck, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Stck<A> operator +(K<Stck, A> ma) =>
            (Stck<A>)ma;
        
        public static Stck<A> operator >> (K<Stck, A> ma, Lower lower) =>
            (Stck<A>)ma;
    }
}
