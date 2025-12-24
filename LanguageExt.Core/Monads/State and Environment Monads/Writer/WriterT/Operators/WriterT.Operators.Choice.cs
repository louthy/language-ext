using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    extension<W, M, A>(K<WriterT<W, M>, A> self)
        where M : Monad<M>, Choice<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static WriterT<W, M, A> operator |(K<WriterT<W, M>, A> lhs, K<WriterT<W, M>, A> rhs) =>
            new (output => lhs.As().runWriter(output) | rhs.As().runWriter(output));

        /// <summary>
        /// Choice operator.  Usually means if the first argument succeeds, return it, otherwise return the second
        /// argument.
        /// </summary>
        /// <param name="lhs">Left hand side operand</param>
        /// <param name="rhs">Right hand side operand</param>
        /// <returns></returns>
        public static WriterT<W, M, A> operator |(K<WriterT<W, M>, A> lhs, Pure<A> rhs) =>
            new (output => lhs.As().runWriter(output) | rhs.Map(x => (x, env: output)));
    }
}
