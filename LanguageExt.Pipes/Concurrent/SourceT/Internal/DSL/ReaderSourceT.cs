using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ReaderSourceT<M, A>(Channel<K<M, A>> Channel) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ReaderSourceTIterator<M, A>(Channel);

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer)
    {
        return reduce(state);

        K<M, S> reduce(S nstate) =>
            M.LiftIO(IO.liftAsync(async e =>
                                  {
                                      if (await Channel.Reader.WaitToReadAsync(e.Token))
                                      {
                                          var value = await Channel.Reader.ReadAsync(e.Token);
                                          return value.Bind(v => reducer(nstate, v));
                                      }
                                      else
                                      {
                                          return M.Pure(nstate);
                                      }
                                  }))
             .Flatten();
    }
}
