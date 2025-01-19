using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class PipeTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ProducerT`.
    /// </summary>
    public static ProducerT<OUT, M, A> ToProducer<OUT, M, A>(this PipeT<Unit, OUT, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);

    /// <summary>
    /// Transformation from `PipeT` to `ConsumerT`.
    /// </summary>
    public static ConsumerT<IN, M, A> ToConsumer<IN, M, A>(this PipeT<IN, Void, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);

    /// <summary>
    /// Transformation from `PipeT` to `EffectT`.
    /// </summary>
    public static EffectT<M, A> ToEffect<M, A>(this PipeT<Unit, Void, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);
}
    
