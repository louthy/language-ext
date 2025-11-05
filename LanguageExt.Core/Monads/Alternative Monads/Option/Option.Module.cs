using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public partial class Option
{
    /// <summary>
    /// None
    /// </summary>
    public static readonly Fail<Unit> None = new (default);

    /// <summary>
    /// Construct an Option of A in a Some state
    /// </summary>
    /// <param name="value">Value to bind, must be non-null</param>
    /// <returns>Option of A</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<A> Some<A>(A value) =>
        new (value);
}
