using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace LanguageExt.Pipes.Concurrent;

record SourcePure<A>(A Value) : Source<A>
{
    public override SourceIterator<A> GetIterator() =>
        new SingletonSourceIterator<A>(Value);
}

record SourceEmpty<A> : Source<A>
{
    public static readonly Source<A> Default = new SourceEmpty<A>();

    public override Source<B> Map<B>(Func<A, B> f) =>
        SourceEmpty<B>.Default;

    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        SourceEmpty<B>.Default;

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        SourceEmpty<B>.Default;

    public override SourceIterator<A> GetIterator() =>
        EmptySourceIterator<A>.Default;
}

record SourceReader<A>(Channel<A> Channel, string Label) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new ReaderSourceIterator<A>(Channel.Reader, Label);
}

record SourceMap<A, B>(Source<A> Source, Func<A, B> F) : Source<B>
{
    public override Source<C> Map<C>(Func<B, C> f) =>
        new SourceMap<A, C>(Source, x => f(F(x)));

    public override Source<C> Bind<C>(Func<B, Source<C>> f) => 
        new SourceBind<A, C>(Source, x => f(F(x)));

    public override SourceIterator<B> GetIterator() => 
        new MapSourceIterator<A, B>(Source.GetIterator(), F);
}

record SourceApply<A, B>(Source<A> Source, Source<Func<A, B>> FF) : Source<B>
{
    public override SourceIterator<B> GetIterator() => 
        new ApplySourceIterator<A, B>(Source.GetIterator(), FF.GetIterator());
}

record SourceBind<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    public override SourceIterator<B> GetIterator() => 
        new BindSourceIterator<A,B>(Source.GetIterator(), x => F(x).GetIterator());
}

record SourceCombine<A>(Seq<Source<A>> Sources) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new CombineSourceIterator<A>(Sources.Map(x => x.GetIterator()));
}

record SourceChoose<A>(Source<A> Left, Source<A> Right) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new ChooseSource<A>(Left.GetIterator(), Right.GetIterator());
}

record SourceZip2<A, B>(Source<A> SourceA, Source<B> SourceB) : Source<(A First, B Second)>
{
    public override SourceIterator<(A First, B Second)> GetIterator() => 
        new Zip2SourceIterator<A, B>(SourceA.GetIterator(), SourceB.GetIterator());
}

record SourceZip3<A, B, C>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC) : Source<(A First, B Second, C Third)>
{
    public override SourceIterator<(A First, B Second, C Third)> GetIterator() => 
        new Zip3SourceIterator<A, B, C>(SourceA.GetIterator(), SourceB.GetIterator(), SourceC.GetIterator());

}

record SourceZip4<A, B, C, D>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC, Source<D> SourceD) 
    : Source<(A First, B Second, C Third, D Fourth)>
{
    public override SourceIterator<(A First, B Second, C Third, D Fourth)> GetIterator() => 
        new Zip4SourceIterator<A, B, C, D>(SourceA.GetIterator(), SourceB.GetIterator(), SourceC.GetIterator(), SourceD.GetIterator());
}

record SourceIteratorSync<A>(IEnumerable<A> Items) : Source<A>
{
    public override SourceIterator<A> GetIterator() =>
        new IteratorSyncSourceIterator<A>{ Src = Items.GetIterator() };
}

record SourceIteratorAsync<A>(IAsyncEnumerable<A> Items) : Source<A>
{
    public override SourceIterator<A> GetIterator() =>
        new IteratorAsyncSourceIterator<A>{ Src = Items.GetIteratorAsync() };
}

record SourceFoldWhile<S, A>(Schedule Schedule, Func<S, A, S> Folder, Func<(S State, A Value), bool> Pred, S State, Source<A> Src)
    : Source<S>
{
    public override SourceIterator<S> GetIterator() => 
        new FoldWhileSourceIterator<S,A>(Schedule, Folder, Pred, State, Src.GetIterator());
}

record SourceFoldUntil<S, A>(Schedule Schedule, Func<S, A, S> Folder, Func<(S State, A Value), bool> Pred, S State, Source<A> Src)
    : Source<S>
{
    public override SourceIterator<S> GetIterator() => 
        new FoldUntilSourceIterator<S,A>(Schedule, Folder, Pred, State, Src.GetIterator());
}

record SourceFilter<A>(Source<A> Source, Func<A, bool> Pred) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new FilterSourceIterator<A>(Source.GetIterator(), Pred);
}
