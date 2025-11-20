using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

public static class ConduitTExtensions
{
    public static ConduitT<M, A, B> As<M, A, B>(this K<ConduitT<M, A>, B> ma) 
        where M : MonadIO<M>, Monad<M>, Alternative<M> =>
        (ConduitT<M, A, B>)ma;
    
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
