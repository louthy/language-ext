#nullable enable
namespace LanguageExt.Transducers;

record FilterTransducer1<A, B>(Transducer<A, B> F, Transducer<B, bool> Predicate) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) => 
        F.Transform(
            Reducer.from<B, S>((st, s, v) => 
                Predicate.Transform(
                    Reducer.from<bool, S>((st2, s2, flag) =>
                        flag 
                            ? reduce.Run(st2, s2, v) 
                            : TResult.Complete(s2))).Run(st, s, v)));
            
    public override string ToString() =>  
        "filter";
}

record FilterSumTransducer1<X, A, B>(Transducer<A, Sum<X, B>> F, Transducer<B, bool> Predicate) : Transducer<A, Sum<X, B>>
{
    public override Reducer<A, S> Transform<S>(Reducer<Sum<X, B>, S> reduce) =>
        F.Transform(
            Reducer.from<Sum<X, B>, S>((st, s, v) =>
                v switch
                {
                    SumRight<X, B> r => Predicate.Transform(
                        Reducer.from<bool, S>((st2, s2, flag) =>
                            flag
                                ? reduce.Run(st2, s2, v)
                                : TResult.Complete(s2))).Run(st, s, r.Value),

                    _ => TResult.Complete(s)
                }));
            
    public override string ToString() =>  
        "filter";
}
