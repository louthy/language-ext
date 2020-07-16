using System.Threading;

namespace LanguageExt
{
    public interface Cancellable
    {
        CancellationToken CancelToken { get; }
    }
}
