using System.Threading;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Access the cancellation-token from the IO environment
    /// </summary>
    /// <returns>CancellationToken</returns>
    public static readonly IO<CancellationToken> cancelToken =
        IO.token;
}
