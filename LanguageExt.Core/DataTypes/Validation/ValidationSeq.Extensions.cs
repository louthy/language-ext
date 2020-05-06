using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationSeqExtensions
    {
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> Flatten<FAIL, SUCCESS>(this Validation<FAIL, Validation<FAIL, SUCCESS>> self) =>
            self.Bind(identity);
    }
}
