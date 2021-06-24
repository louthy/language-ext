using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys
{
    public static class Enc<RT>
        where RT : struct, HasEncoding<RT> 
    {
        /// <summary>
        /// Encoding
        /// </summary>
        /// <typeparam name="RT">Runtime environment</typeparam>
        /// <returns>Encoding</returns>
        public static Eff<RT, System.Text.Encoding> encoding 
        {
            [MethodImpl(AffOpt.mops)]
            get => Eff<RT, System.Text.Encoding>(static rt => rt.Encoding);
        }
    }
}
