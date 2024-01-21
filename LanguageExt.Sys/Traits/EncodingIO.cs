using System.Text;
using System.Threading;
using LanguageExt.Attributes;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    /// <summary>
    /// Type-class giving a struct the trait of supporting text encoding IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasEncoding<out RT> : HasIO<RT, Error>
        where RT : struct, HasEncoding<RT>
    {
        /// <summary>
        /// Access the text encoding
        /// </summary>
        Encoding Encoding { get; }
    }
}
