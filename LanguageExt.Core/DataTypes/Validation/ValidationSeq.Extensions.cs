using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
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
        
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<MonoidFail, FAIL, SUCCESS> Flatten<MonoidFail, FAIL, SUCCESS>(this Validation<MonoidFail, FAIL, Validation<MonoidFail, FAIL, SUCCESS>> self)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            self.Bind(identity);
        
    }
}
