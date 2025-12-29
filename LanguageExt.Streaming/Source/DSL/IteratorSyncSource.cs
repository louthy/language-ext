using System.Collections.Generic;

namespace LanguageExt;

record IteratorSyncSource<A>(IEnumerable<A> Items) : Source<A>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) =>
        IO.liftVAsync(async e =>
                      {
                          foreach (var item in Items)
                          {
                              if (e.Token.IsCancellationRequested) return Reduced.Done(state);

                              switch (await reducer(state, item).RunAsync(e))
                              {
                                  case { Continue: true, Value: var value }:
                                      state = value;
                                      break;

                                  case { Value: var value }:
                                      return Reduced.Done(value);
                              }
                          }

                          return Reduced.Continue(state);
                      });
}
