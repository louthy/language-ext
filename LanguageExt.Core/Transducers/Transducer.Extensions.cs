using LanguageExt.Traits;

namespace LanguageExt;

public static class TransducerExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static Transducer<A, B> As<A, B>(this K<TransduceFrom<A>, B> ma) =>
        (Transducer<A, B>)ma;
    
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static Transducer<A, B> As<A, B>(this K<TransduceTo<B>, A> ma) =>
        (Transducer<A, B>)ma;    
}
