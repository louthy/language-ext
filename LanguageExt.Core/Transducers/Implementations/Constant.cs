#nullable enable
namespace LanguageExt.Transducers;

record ConstantTransducer<A, B>(B Value) : Transducer<A, B>
{
    public Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(Value, reduce);

    record Reduce<S>(B Value, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A _) =>
            Reducer.Run(st, s, Value);
    }

    public Transducer<A, B> Morphism =>
        this;

    public override string ToString() =>
        "const";
}
