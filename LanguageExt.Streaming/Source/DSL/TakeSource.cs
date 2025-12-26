using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record TakeSource<A>(Source<A> Source, int Count) : Source<A>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        var remaining = Count;
        return Source.ReduceAsync(state,
                                  async (s, a) =>
                                  {
                                      if (remaining < 1) return Reduced.Done(s);
                                      remaining--;
                                      return remaining == 0
                                                ? await reducer(s, a) switch
                                                  {
                                                      { Value: var v } => Reduced.Done(v)
                                                  }
                                                : await reducer(s, a);
                                  },
                                  token);
    }
}
