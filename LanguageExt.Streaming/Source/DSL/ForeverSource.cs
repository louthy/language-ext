namespace LanguageExt;

record ForeverSource<A>(A Value) : Source<A>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) =>
        IO.liftVAsync(async e =>
                      {
                          while (!e.Token.IsCancellationRequested)
                          {
                              switch (await reducer(state, Value).RunAsync(e))
                              {
                                  case { Continue: true, Value: var value }:
                                      state = value;
                                      break;

                                  case { Value: var value }:
                                      return Reduced.Done(value);
                              }
                          }

                          return Reduced.Done(state);
                      });
}
