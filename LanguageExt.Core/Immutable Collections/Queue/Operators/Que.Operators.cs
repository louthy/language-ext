using LanguageExt.Traits;

namespace LanguageExt;

public static partial class QueExtensions
{
    extension<A>(K<Que, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Que<A> operator +(K<Que, A> ma) =>
            (Que<A>)ma;
        
        public static Que<A> operator >> (K<Que, A> ma, Lower lower) =>
            (Que<A>)ma;
    }
}
