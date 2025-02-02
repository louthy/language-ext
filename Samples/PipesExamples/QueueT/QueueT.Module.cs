using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `QueueT` streaming producer monad-transformer
/// </summary>
public static class QueueT
{
    /// <summary>
    /// Create a new `QueueT`
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static QueueT<OUT, M, Unit> create<M, OUT>()
        where M : Monad<M>
    {
        var c = new Channel<OUT>();
        var p = ProducerT.yieldAll<M, OUT>(c);
        return new QueueT<OUT, M, Unit>(c, p);
    }
}
