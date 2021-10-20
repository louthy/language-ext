using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> flatten<FAIL, SUCCESS>(this Validation<FAIL, Validation<FAIL, SUCCESS>> self) =>
            self.Flatten();
    }
}
