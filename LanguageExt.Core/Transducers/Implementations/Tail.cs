#nullable enable
namespace LanguageExt;

record TailTransducer<A, B>(Transducer<A, B> Recursive) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        Recursive.Transform(reduce);
                        
    public override string ToString() =>  
        $"tail";
}
