using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys
{
    public static class Enc
    {
        /// <summary>
        /// Encoding
        /// </summary>
        /// <typeparam name="RT">Runtime environment</typeparam>
        /// <returns>Encoding</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, System.Text.Encoding> encoding<RT>()
            where RT : struct, HasEncoding<RT> =>
                Eff<RT, System.Text.Encoding>(static rt => rt.Encoding);
    }
}
