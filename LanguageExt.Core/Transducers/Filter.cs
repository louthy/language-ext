namespace LanguageExt.Transducers;

record FilterTransducer1<A, B>(Transducer<A, B> F, Transducer<B, bool> Predicate) : Transducer<A, B>
{
    public Transducer<A, B> Morphism =>
        this;
    
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) => 
        F.Transform(
            Reducer.from<S, B>((st, s, v) => 
                Predicate.Transform(
                    Reducer.from<S, bool>((st2, s2, flag) =>
                        flag 
                            ? reduce.Run(st2, s2, v) 
                            : TResult.Complete(s2))).Run(st, s, v)));
}

record FilterSumTransducer1<X, A, B>(Transducer<A, Sum<X, B>> F, Transducer<B, bool> Predicate) : Transducer<A, Sum<X, B>>
{
    public Transducer<A, Sum<X, B>> Morphism =>
        this;

    public Reducer<S, A> Transform<S>(Reducer<S, Sum<X, B>> reduce) =>
        F.Transform(
            Reducer.from<S, Sum<X, B>>((st, s, v) =>
                v switch
                {
                    SumRight<X, B> r => Predicate.Transform(
                        Reducer.from<S, bool>((st2, s2, flag) =>
                            flag
                                ? reduce.Run(st2, s2, v)
                                : TResult.Complete(s2))).Run(st, s, r.Value),

                    _ => TResult.Complete(s)
                }));
}
