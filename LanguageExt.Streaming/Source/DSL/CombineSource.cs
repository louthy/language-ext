namespace LanguageExt;

record CombineSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) =>
        IO.liftVAsync(async e =>
          {
              foreach (var source in Sources)
              {
                  if (e.Token.IsCancellationRequested) return Reduced.Done(state);
                  switch (await source.ReduceInternal(state, reducer).RunAsync(e))
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
