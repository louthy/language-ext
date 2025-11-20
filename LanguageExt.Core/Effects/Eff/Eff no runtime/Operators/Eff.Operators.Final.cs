namespace LanguageExt.Traits;

public static partial class EffExtensions
{
    extension<X, A>(K<Eff, A> _)
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static Eff<A> operator |(K<Eff, A> lhs, Finally<Eff, X> rhs) =>
            lhs.Finally(rhs.Operation).As();
    }
}
