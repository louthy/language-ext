using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class PipeExtensions
{
    extension<RT, IN, OUT, A>(K<Pipe<RT, IN, OUT>, A>)
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static Pipe<RT, IN, OUT, A> operator +(K<Pipe<RT, IN, OUT>, A> ma) =>
           (Pipe<RT, IN, OUT, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static Pipe<RT, IN, OUT, A> operator >>(K<Pipe<RT, IN, OUT>, A> ma, Lower lower) =>
           +ma;
    }
}
