using static LanguageExt.Prelude;

namespace LanguageExt;

record TakeSource<A>(Source<A> Source, int Count) : Source<A>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) =>
        from remain in atomIO(Count)
        from value in Source.ReduceInternal(
            state,
            (s, a) => IO.liftVAsync(async e =>
                                    {
                                        if (remain.Value < 1) return Reduced.Done(s);
                                        remain.Swap(v => v - 1);
                                        return remain.Value == 0
                                                   ? await reducer(s, a).RunAsync(e) switch
                                                     {
                                                         { Value: var v } => Reduced.Done(v)
                                                     }
                                                   : await reducer(s, a).RunAsync(e);
                                    }))
        select value;
}
