using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        public static ChronicleT<Ch, M, A> operator |(K<ChronicleT<Ch, M>, A> lhs, K<ChronicleT<Ch, M>, A> rhs) =>
            +lhs.Choose(rhs);

        public static ChronicleT<Ch, M, A> operator |(K<ChronicleT<Ch, M>, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(ChronicleT.chronicle<Ch, M, A>(rhs.ToThese<Ch>()));
    }
}
