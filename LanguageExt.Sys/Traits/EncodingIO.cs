using System.Text;
using System.Threading;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    /// <summary>
    /// Type-class giving a struct the trait of supporting text encoding IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasEncoding<RT> : HasCancel<RT>
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Access the text encoding
        /// </summary>
        Encoding Encoding { get; }
    }
}
