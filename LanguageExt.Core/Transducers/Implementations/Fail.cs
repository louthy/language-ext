#nullable enable
using LanguageExt.Common;

namespace LanguageExt.Transducers;

record FailTransducer<A, B>(Error Error) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        Reducer.from<A, S>((_, _, _) => TResult.Fail<S>(Error));
        
    public override string ToString() =>  
        "fail";
}
