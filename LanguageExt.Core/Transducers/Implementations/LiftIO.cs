#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Transducers;

record LiftIOTransducer1<A, B>(Func<CancellationToken, A, Task<TResult<B>>> F) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, A, Task<TResult<B>>> F, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            TaskAsync<A>.Run(F, x, st.Token).Reduce(st, s, Reducer);
    }

    public override string ToString() =>
        "liftIO";
}

record LiftIOTransducer2<A>(Func<CancellationToken, Task<TResult<A>>> F) : Transducer<Unit, A>
{
    public override Reducer<Unit, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, Task<TResult<A>>> F, Reducer<A, S> Reducer) : Reducer<Unit, S>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            TaskAsync<A>.Run(F, st.Token).Reduce(st, s, Reducer);
    }

    public override string ToString() =>
        "liftIO";
}

record LiftIOTransducer3<A, B>(Func<CancellationToken, A, Task<B>> F) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, A, Task<B>> F, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            TaskAsync<A>.Run(F, x, st.Token).Reduce(st, s, Reducer);
    }

    public override string ToString() =>
        "liftIO";
}

record LiftIOTransducer4<A>(Func<CancellationToken, Task<A>> F) : Transducer<Unit, A>
{
    public override Reducer<Unit, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, Task<A>> F, Reducer<A, S> Reducer) : Reducer<Unit, S>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            TaskAsync<A>.Run(F, st.Token).Reduce(st, s, Reducer);
    }

    public override string ToString() =>
        "liftIO";
}
