using System;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

public static class SinkTExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static SinkT<M, A> As<M, A>(this K<SinkT<M>, A> ma) 
        where M : MonadIO<M> =>
        (SinkT<M, A>)ma;
    
    /// <summary>
    /// Convert the `Sink` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public static Consumer<RT, A, Unit> ToConsumer<RT, A>(this SinkT<Eff<RT>, A> ma) =>
        ma.ToConsumerT();
}
