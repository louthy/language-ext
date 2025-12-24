using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ArrExtensions
{
    extension<A>(K<Arr, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Arr<A> operator +(K<Arr, A> ma) =>
            (Arr<A>)ma;
        
        public static Arr<A> operator >> (K<Arr, A> ma, Lower lower) =>
            (Arr<A>)ma;
    }
}
