using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public abstract record SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public abstract ReadResult<M, A> Read();
    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);

    public SourceTIterator<M, B> Map<B>(Func<A, B> f) =>
        new MapSourceTIterator<M, A, B>(this, f);

    public SourceTIterator<M, B> Bind<B>(Func<A, SourceTIterator<M, B>> f) =>
        new BindSourceTIterator<M, A, B>(this, f);

    public SourceTIterator<M, B> ApplyBack<B>(SourceTIterator<M, Func<A, B>> ff) =>
        new ApplySourceTIterator<M, A, B>(ff, this);

    public SourceTIterator<M, A> Choose(SourceTIterator<M, A> rhs) =>
        ChooseSourceT.makeIterator([this, rhs]);
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
    {
        return M.LiftIO(IO.lift<K<M, S>>(e => go2(state, reducer, this, e.Token))).Flatten();

        K<M, S> go2(S state, ReducerM<M, A, S> reducer, SourceTIterator<M, A> iter, CancellationToken token) =>
            iter.ReadyToRead(token).GetAwaiter().GetResult()
                ? iter.Read() switch
                  {
                      ReadM<M, A> (var ma) =>
                          ma.Bind(a => reducer(state, a).Bind(s => go2(s, reducer, iter, token)))
                            .Choose(() => M.Pure(state)),

                      ReadIter<M, A> (var miter) =>
                          miter.Bind(i => go2(state, reducer, i, token).Bind(s => go2(s, reducer, iter, token)))
                  }
                : M.Pure(state);
    }

    public K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return M.LiftIO(IO.lift<K<M, S>>(e => go2(state, reducer, this, e.Token))).Flatten();

        K<M, S> go2(S state, ReducerM<M, K<M, A>, S> reducer, SourceTIterator<M, A> iter, CancellationToken token) =>
            iter.ReadyToRead(token).GetAwaiter().GetResult()
                ? iter.Read() switch
                  {
                      ReadM<M, A> (var ma) =>
                          reducer(state, ma).Bind(s => go2(s, reducer, iter, token))
                                            .Choose(() => M.Pure(state)),

                      ReadIter<M, A> (var miter) =>
                          miter.Bind(i => go2(state, reducer, i, token).Bind(s => go2(s, reducer, iter, token)))
                  }
                : M.Pure(state);
    }
}
