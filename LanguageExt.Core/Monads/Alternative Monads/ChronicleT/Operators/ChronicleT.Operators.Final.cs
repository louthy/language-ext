namespace LanguageExt.Traits;

public static class ChronicleTExtensions
{
    extension<X, Ch, M, A>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>, Final<M>
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static ChronicleT<Ch, M, A> operator |(K<ChronicleT<Ch, M>, A> lhs, Finally<M, X> rhs) =>
            new (semi => lhs.As().runChronicleT(semi).Finally(rhs.Operation));
    }
}
