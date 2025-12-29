using System;

namespace LanguageExt;

record ApplySource<A, B>(Source<Func<A, B>> FF, Source<A> FA) : Source<B>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<B, S> reducer) => 
        FF.Zip(FA).Map(p => p.First(p.Second)).ReduceInternal(state, reducer);
}

record ApplySource2<A, B>(Source<Func<A, B>> FF, Memo<Source, A> FA) : Source<B>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<B, S> reducer) => 
        FF.Zip(FA.Value.As()).Map(p => p.First(p.Second)).ReduceInternal(state, reducer);
}
