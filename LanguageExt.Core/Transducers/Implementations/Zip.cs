using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Transducers;

record ZipTransducer2<E, A, B>(Transducer<E, A> First, Transducer<E, B> Second)
    : Transducer<E, (A First, B Second)>
{
    public Transducer<E, (A First, B Second)> Morphism =>
        this;

    public Reducer<S, E> Transform<S>(Reducer<S, (A First, B Second)> reduce) =>
        new Reduce1<S>(First, Second, reduce);

    record Reduce1<S>(Transducer<E, A> First, Transducer<E, B> Second, Reducer<S, (A First, B Second)> Reduce) : Reducer<S, E>
    {
        public override TResult<S> Run(TState state, S stateValue, E value) =>
            First.Transform(
                     Reducer.from<S, A>((st, s, a) =>
                         Second.Transform(
                                   Reducer.from<S, B>((st2, s2, b) =>
                                       Reduce.Run(st2, s2, (a, b))))
                               .Run(st, s, value)))
                 .Run(state, stateValue, value);
    }
}

record ZipSumTransducer2<E, X, A, B>(Transducer<E, Sum<X, A>> First, Transducer<E, Sum<X, B>> Second)
    : Transducer<E, Sum<X, (A First, B Second)>>
{
    public Transducer<E, Sum<X, (A First, B Second)>> Morphism =>
        this;

    public Reducer<S, E> Transform<S>(Reducer<S, Sum<X, (A First, B Second)>> reduce) =>
        new Reduce1<S>(First, Second, reduce);

    record Reduce1<S>(
            Transducer<E, Sum<X, A>> First, 
            Transducer<E, Sum<X, B>> Second, 
            Reducer<S, Sum<X, (A First, B Second)>> Reduce) 
        : Reducer<S, E>
    {
        public override TResult<S> Run(TState state, S stateValue, E value) =>
            First.Transform(
                Reducer.from<S, Sum<X, A>>((st, s, sa) =>
                    sa switch
                    {
                        SumRight<X, A> t1 =>
                            Second.Transform(
                                Reducer.from<S, Sum<X, B>>((st2, s2, sb) =>
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
}
