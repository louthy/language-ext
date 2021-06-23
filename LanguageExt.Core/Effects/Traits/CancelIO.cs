using System.Threading;
using LanguageExt.Attributes;

namespace LanguageExt.Effects.Traits
{
    /// <summary>
    /// Type-class giving a struct the trait of supporting task cancellation IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasCancel<out RT>
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Creates a new runtime from this with a new CancellationTokenSource and token
        /// </summary>
        /// <remarks>This is for sub-systems to run in their own local cancellation contexts</remarks>
        /// <returns>New runtime</returns>
        RT LocalCancel { get; }

        /// <summary>
        /// Directly access the cancellation token
        /// </summary>
        /// <returns>CancellationToken</returns>
        CancellationToken CancellationToken { get; }
        
        /// <summary>
        /// Get the cancellation token source
        /// </summary>
        /// <returns></returns>
        CancellationTokenSource CancellationTokenSource { get; }
    }
}
