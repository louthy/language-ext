#nullable enable

namespace LanguageExt.Transducers;

record ZipTransducer2<E, A, B>(Transducer<E, A> First, Transducer<E, B> Second)
    : Transducer<E, (A First, B Second)>
{
    public Transducer<E, (A First, B Second)> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<(A First, B Second), S> reduce) =>
        new Reduce1<S>(First, Second, reduce);

    record Reduce1<S>(Transducer<E, A> First, Transducer<E, B> Second, Reducer<(A First, B Second), S> Reduce) : Reducer<E, S>
    {
        public override TResult<S> Run(TState state, S stateValue, E value) =>
            First.Transform(
                     Reducer.from<A, S>((st, s, a) =>
                         Second.Transform(
                                   Reducer.from<B, S>((st2, s2, b) =>
                                       Reduce.Run(st2, s2, (a, b))))
                               .Run(st, s, value)))
                 .Run(state, stateValue, value);
    }
                                
    public override string ToString() =>  
        $"zip";
}

record ZipSumTransducer2<E, X, A, B>(Transducer<E, Sum<X, A>> First, Transducer<E, Sum<X, B>> Second)
    : Transducer<E, Sum<X, (A First, B Second)>>
{
    public Transducer<E, Sum<X, (A First, B Second)>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<X, (A First, B Second)>, S> reduce) =>
        new Reduce1<S>(First, Second, reduce);

    record Reduce1<S>(
            Transducer<E, Sum<X, A>> First, 
            Transducer<E, Sum<X, B>> Second, 
            Reducer<Sum<X, (A First, B Second)>, S> Reduce) 
        : Reducer<E, S>
    {
        public override TResult<S> Run(TState state, S stateValue, E value) =>
            First.Transform(
                Reducer.from<Sum<X, A>, S>((st, s, sa) =>
                    sa switch
                    {
                        SumRight<X, A> t1 =>
                            Second.Transform(
                                Reducer.from<Sum<X, B>, S>((st2, s2, sb) =>
                                    sb switch
                                    {
                                        SumRight<X, B> t2 =>
                                            Reduce.Run(st2, s2, Sum<X, (A, B)>.Right((t1.Value, t2.Value))),

                                        SumLeft<X, B> t2 =>
                                            Reduce.Run(st2, s2, Sum<X, (A, B)>.Left(t2.Value)),

                                        _ => TResult.Complete(s2)
                                    })).Run(st, s, value),
                        SumLeft<X, A> t1 =>
                            Reduce.Run(st, s, Sum<X, (A, B)>.Left(t1.Value)),

                        _ => TResult.Complete(s)
                    })).Run(state, stateValue, value);
    }
                                
    public override string ToString() =>  
        $"zip";
}

record ZipTransducer3<E, A, B, C>(
        Transducer<E, A> First, 
        Transducer<E, B> Second,
        Transducer<E, C> Third)
    : Transducer<E, (A First, B Second, C Third)>
{
    public Transducer<E, (A First, B Second, C Third)> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<(A First, B Second, C Third), S> reduce) =>
        new Reduce1<S>(First, Second, Third, reduce);

    record Reduce1<S>(
        Transducer<E, A> First, 
        Transducer<E, B> Second, 
        Transducer<E, C> Third,
        Reducer<(A First, B Second, C Third), S> Reduce) : Reducer<E, S>
    {
        public override TResult<S> Run(TState state, S stateValue, E value) =>
            First.Transform(
                     Reducer.from<A, S>((st, s, a) =>
                         Second.Transform(
                                   Reducer.from<B, S>((st2, s2, b) =>
                                       Third.Transform(
                                                Reducer.from<C, S>((st3, s3, c) =>
                                                    Reduce.Run(st3, s3, (a, b, c))))
                                            .Run(st2, s2, value)))
                               .Run(st, s, value)))
                 .Run(state, stateValue, value);
    }
                                
    public override string ToString() =>  
        $"zip";
}

record ZipSumTransducer3<E, X, A, B, C>(
        Transducer<E, Sum<X, A>> First, 
        Transducer<E, Sum<X, B>> Second,
        Transducer<E, Sum<X, C>> Third
        )
    : Transducer<E, Sum<X, (A First, B Second, C Third)>>
{
    public Transducer<E, Sum<X, (A First, B Second, C Third)>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<X, (A First, B Second, C Third)>, S> reduce) =>
        new Reduce1<S>(First, Second, Third, reduce);

    record Reduce1<S>(
            Transducer<E, Sum<X, A>> First, 
            Transducer<E, Sum<X, B>> Second, 
            Transducer<E, Sum<X, C>> Third, 
            Reducer<Sum<X, (A First, B Second, C Third)>, S> Reduce) 
        : Reducer<E, S>
    {
        public override TResult<S> Run(TState state, S stateValue, E value) =>
            First.Transform(
                Reducer.from<Sum<X, A>, S>((st, s, sa) =>
                    sa switch
                    {
                        SumRight<X, A> t1 =>
                            Second.Transform(
                                Reducer.from<Sum<X, B>, S>((st2, s2, sb) =>
                                    sb switch
                                    {
                                        SumRight<X, B> t2 =>
                                            Third.Transform(
                                                Reducer.from<Sum<X, C>, S>((st3, s3, sc) =>
                                                    sc switch
                                                    {
                                                        SumRight<X, C> t3 =>
                                                            Reduce.Run(st3, s3, Sum<X, (A, B, C)>.Right((t1.Value, t2.Value, t3.Value))),

                                                        SumLeft<X, C> t3 =>
                                                            Reduce.Run(st3, s3, Sum<X, (A, B, C)>.Left(t3.Value)),

                                                        _ => TResult.Complete(s2)
                                                    })).Run(st2, s2, value),
                                            
                                        SumLeft<X, B> t2 =>
                                            Reduce.Run(st2, s2, Sum<X, (A, B, C)>.Left(t2.Value)),

                                        _ => TResult.Complete(s2)
                                    })).Run(st, s, value),
                        SumLeft<X, A> t1 =>
                            Reduce.Run(st, s, Sum<X, (A, B, C)>.Left(t1.Value)),

                        _ => TResult.Complete(s)
                    })).Run(state, stateValue, value);
    }
                                
    public override string ToString() =>  
        $"zip";
}
