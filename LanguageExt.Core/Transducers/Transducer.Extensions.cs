using LanguageExt.Traits;

namespace LanguageExt;

public static class TransducerExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static Transducer<A, B> As<A, B>(this K<Transducer<A>, B> ma) =>
        (Transducer<A, B>)ma;
}
