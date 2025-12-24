using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ReaderExtensions
{
    extension<Env, A>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Reader<Env, A> operator +(K<Reader<Env>, A> ma) =>
            (Reader<Env, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Reader<Env, A> operator >> (K<Reader<Env>, A> ma, Lower lower) =>
            +ma;
    }
}
