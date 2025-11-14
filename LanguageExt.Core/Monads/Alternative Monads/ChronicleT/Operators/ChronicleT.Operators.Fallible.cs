using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        public static ChronicleT<Ch, M, A> operator |(K<ChronicleT<Ch, M>, A> lhs, CatchM<Ch, ChronicleT<Ch, M>, A> rhs) =>
            +lhs.Catch(rhs);

        public static ChronicleT<Ch, M, A> operator |(K<ChronicleT<Ch, M>, A> lhs, Fail<Ch> rhs) =>
            +lhs.Catch(rhs);
    }
}
