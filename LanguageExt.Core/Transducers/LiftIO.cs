#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Transducers;

record LiftIOTransducer1<A, B>(Func<CancellationToken, A, Task<TResult<B>>> F) : Transducer<A, B>
{
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, A, Task<TResult<B>>> F, Reducer<S, B> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            TaskAsync<A>.Run(F, x, st.Token).Reduce(st, s, Reducer);
    }
    
    public Transducer<A, B> Morphism =>
        this;

    public override string ToString() =>
        "liftIO";
}

record LiftIOTransducer2<A>(Func<CancellationToken, Task<TResult<A>>> F) : Transducer<Unit, A>
{
    public Reducer<S, Unit> Transform<S>(Reducer<S, A> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, Task<TResult<A>>> F, Reducer<S, A> Reducer) : Reducer<S, Unit>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            TaskAsync<A>.Run(F, st.Token).Reduce(st, s, Reducer);
    }
    
    public Transducer<Unit, A> Morphism =>
        this;

    public override string ToString() =>
        "liftIO";
}

record LiftIOTransducer3<A, B>(Func<CancellationToken, A, Task<B>> F) : Transducer<A, B>
{
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, A, Task<B>> F, Reducer<S, B> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            TaskAsync<A>.Run(F, x, st.Token).Reduce(st, s, Reducer);
    }
    
    public Transducer<A, B> Morphism =>
        this;

    public override string ToString() =>
        "liftIO";
}

record LiftIOTransducer4<A>(Func<CancellationToken, Task<A>> F) : Transducer<Unit, A>
{
    public Reducer<S, Unit> Transform<S>(Reducer<S, A> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<CancellationToken, Task<A>> F, Reducer<S, A> Reducer) : Reducer<S, Unit>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            TaskAsync<A>.Run(F, st.Token).Reduce(st, s, Reducer);
    }
    
    public Transducer<Unit, A> Morphism =>
        this;

    public override string ToString() =>
        "liftIO";
}
