using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static ChronicleT<Ch, M, A> operator +(K<ChronicleT<Ch, M>, A> ma) =>
            (ChronicleT<Ch, M, A>)ma;
    }
}
