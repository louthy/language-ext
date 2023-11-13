#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageExt.Transducers;

record StreamEnumerableTransducer<A> : Transducer<IEnumerable<A>, A>
{
    public static Transducer<IEnumerable<A>, A> Default = new StreamEnumerableTransducer<A>();
    
    public Transducer<IEnumerable<A>, A> Morphism =>
        this;

    public Reducer<IEnumerable<A>, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce1<S>(reduce);

    record Reduce1<S>(Reducer<A, S> Reduce) : Reducer<IEnumerable<A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, IEnumerable<A> items)
        {
            foreach (var item in items)
            {
                var result = Go(Reduce.Run(state, stateValue, item));
                switch(result)
                {
                    case TContinue<S> {Value: not null} r:
                        stateValue = r.Value;
                        break;

                    default:
                        return result;
                }
            }
            return TResult.Continue(stateValue);
        }

        TResult<S> Go(TResult<S> result)
        {
            while (true)
            {
                switch (result)
                {
                    case TRecursive<S> v:
                        result = v.Run();
                        break;
                    
                    default:
                        return result;
                }
            }
        }
    }
}

record StreamSeqTransducer<A> : Transducer<Seq<A>, A>
{
    public static Transducer<Seq<A>, A> Default = new StreamSeqTransducer<A>();
    
    public Transducer<Seq<A>, A> Morphism =>
        this;

    public Reducer<Seq<A>, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce1<S>(reduce);

    record Reduce1<S>(Reducer<A, S> Reduce) : Reducer<Seq<A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Seq<A> items)
        {
            foreach (var item in items)
            {
                var result = Go(Reduce.Run(state, stateValue, item));
                switch(result)
                {
                    case TContinue<S> {Value: not null} r:
                        stateValue = r.Value;
                        break;

                    default:
                        return result;
                }
            }
            return TResult.Continue(stateValue);
        }

        TResult<S> Go(TResult<S> result)
        {
            while (true)
            {
                switch (result)
                {
                    case TRecursive<S> v:
                        result = v.Run();
                        break;
                    
                    default:
                        return result;
                }
            }
        }
    }
}

record StreamAsyncEnumerableTransducer<A> : Transducer<IAsyncEnumerable<A>, A>
{
    public static Transducer<IAsyncEnumerable<A>, A> Default = new StreamAsyncEnumerableTransducer<A>();
    
    public Transducer<IAsyncEnumerable<A>, A> Morphism =>
        this;

    public Reducer<IAsyncEnumerable<A>, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce1<S>(reduce);

    record Reduce1<S>(Reducer<A, S> Reduce) : Reducer<IAsyncEnumerable<A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, IAsyncEnumerable<A> items) =>
            TaskAsync<Unit>.Run<S>((_, _) => GoAsync(state, stateValue, items), default, state.Token);

        async Task<TResult<S>> GoAsync(TState state, S stateValue, IAsyncEnumerable<A> items)
        {
            await foreach (var item in items.ConfigureAwait(false))
            {
                var result = Go(Reduce.Run(state, stateValue, item));
                switch(result)
                {
                    case TContinue<S> {Value: not null} r:
                        stateValue = r.Value;
                        break;

                    default:
                        return result;
                }
            }
            return TResult.Continue(stateValue);
        }

        TResult<S> Go(TResult<S> result)
        {
            while (true)
            {
                switch (result)
                {
                    case TRecursive<S> v:
                        result = v.Run();
                        break;
                    
                    default:
                        return result;
                }
            }
        }
    }
}
