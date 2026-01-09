using System.Threading;

namespace LanguageExt;

/// <summary>
/// Used by the structures that typically yield a single value or none (like `Identity`, `Option`, `Fin`,
/// `Either`, etc.) when implementing the `FoldStep` functionality.
/// 
/// The `HasRun` flag gets flipped by `ShouldYield()` the first time it is called, indicating that the
/// structure that contains the bound value should yield it (if the structure is not in its alternative
/// state). 
/// </summary>
public ref struct SingletonFoldState
{
    public int HasRun;
}

public static class SingletonFoldStateExtensions
{
    extension(ref SingletonFoldState self)
    {
        /// <summary>
        /// Returns true if the alternative-monad should yield its bound value (if not in its alternative state).
        /// </summary>
        public bool ShouldYield() =>
            Interlocked.CompareExchange(ref self.HasRun, 1, 0) == 0;
    }
}
