using System;

namespace LanguageExt;

record BindSource<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<B, S> reducer) => 
        Source.ReduceInternal(state, (s, x) => F(x).ReduceInternal(s, reducer));
}
