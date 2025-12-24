using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class ConsumerExtensions
{
    extension<RT, IN, A>(K<Consumer<RT, IN>, A>)
    {
        /// <summary>
        /// Downcast
        /// </summary>
        public static Consumer<RT, IN, A> operator +(K<Consumer<RT, IN>, A> ma) =>
           (Consumer<RT, IN, A>) ma;
        
        /// <summary>
        /// Downcast
        /// </summary>
        public static Consumer<RT, IN, A> operator >>(K<Consumer<RT, IN>, A> ma, Lower lower) =>
           +ma;
    }
}
