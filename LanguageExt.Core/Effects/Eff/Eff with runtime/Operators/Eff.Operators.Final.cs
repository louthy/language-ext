namespace LanguageExt.Traits;

public static partial class EffExtensions
{
    extension<RT, X, A>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Run a `finally` operation after the main operation regardless of whether it succeeds or not.
        /// </summary>
        /// <param name="lhs">Primary operation</param>
        /// <param name="rhs">Finally operation</param>
        /// <returns>Result of primary operation</returns>
        public static Eff<RT, A> operator |(K<Eff<RT>, A> lhs, Finally<Eff<RT>, X> rhs) =>
            lhs.Finally(rhs.Operation).As();
    }
}
