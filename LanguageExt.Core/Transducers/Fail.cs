using LanguageExt.Common;

namespace LanguageExt.Transducers;

record FailTransducer<A, B>(Error Error) : Transducer<A, B>
{
    public Transducer<A, B> Morphism =>
        this;
    
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        Reducer.from<S, A>((_, _, _) => TResult.Fail<S>(Error));
}
