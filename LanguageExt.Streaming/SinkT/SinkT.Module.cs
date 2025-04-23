using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class SinkT
{
    /// <summary>
    /// Create a sink from a `System.Threading.Channels.Channel`.
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Constructed sink</returns>
    public static SinkT<M, A> lift<M, A>(Channel<K<M, A>> channel) 
        where M : MonadIO<M> =>
        new SinkTWriter<M, A>(channel);
}
