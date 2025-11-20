using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Sink
{
    /// <summary>
    /// Create a sink from a `System.Threading.Channels.Channel`.
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Constructed sink</returns>
    public static Sink<A> lift<A>(Channel<A> channel) =>
        new SinkWriter<A>(channel);
}
