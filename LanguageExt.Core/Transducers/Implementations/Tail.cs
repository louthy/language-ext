#nullable enable
namespace LanguageExt.Transducers;

record TailTransducer<A, B>(Transducer<A, B> Recursive) : Transducer<A, B>
{
    public Transducer<A, B> Morphism =>
        Recursive;

    public Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        Recursive.Transform(reduce);
                        
    public override string ToString() =>  
        $"tail";
}
