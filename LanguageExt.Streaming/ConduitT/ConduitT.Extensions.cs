using LanguageExt.Pipes;

namespace LanguageExt;

public static class ConduitTExtensions
{
    /// <summary>
    /// Convert the conduit's `Sink` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public static Consumer<RT, A, Unit> ToConsumer<RT, A, B>(this ConduitT<Eff<RT>, A, B> conduit) =>
        new (conduit.ToConsumerT().Proxy);

    /// <summary>
    /// Convert the conduit's `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public static Producer<RT, B, Unit> ToProducer<RT, A, B>(this ConduitT<Eff<RT>, A, B> conduit) =>
        new(conduit.ToProducerT().Proxy);
}
