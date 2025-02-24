using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace LanguageExt.Pipes.Concurrent;

record PureSource<A>(A Value) : Source<A>
{
    public override SourceIterator<A> GetIterator() =>
        new SingletonSourceIterator<A>(Value);
}

record EmptySource<A> : Source<A>
{
    public static readonly Source<A> Default = new EmptySource<A>();

    public override Source<B> Map<B>(Func<A, B> f) =>
        EmptySource<B>.Default;

    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        EmptySource<B>.Default;

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        EmptySource<B>.Default;

    public override SourceIterator<A> GetIterator() =>
        EmptySourceIterator<A>.Default;
}

record ReaderSource<A>(Channel<A> Channel, string Label) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new ReaderSourceIterator<A>(Channel.Reader, Label);
}

/*
record MapSource<A, B>(Source<A> Source, Func<A, B> F) : Source<B>
{
    public override Source<C> Map<C>(Func<B, C> f) =>
        new MapSource<A, C>(Source, x => f(F(x)));

    public override Source<C> Bind<C>(Func<B, Source<C>> f) => 
        new BindSource<A, C>(Source, x => f(F(x)));

    public override SourceIterator<B> GetIterator() => 
        new MapSourceIterator<A, B>(Source.GetIterator(), F);
}

record ApplySource<A, B>(Source<A> Source, Source<Func<A, B>> FF) : Source<B>
{
    public override SourceIterator<B> GetIterator() => 
        new ApplySourceIterator<A, B>(Source.GetIterator(), FF.GetIterator());
}

record BindSource<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    public override SourceIterator<B> GetIterator() => 
        new BindSourceIterator<A,B>(Source.GetIterator(), x => F(x).GetIterator());
}
*/

record CombineSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new CombineSourceIterator<A>(Sources.Map(x => x.GetIterator()));
}

record SourceChoose<A>(Source<A> Left, Source<A> Right) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new ChooseSource<A>(Left.GetIterator(), Right.GetIterator());
}

record Zip2Source<A, B>(Source<A> SourceA, Source<B> SourceB) : Source<(A First, B Second)>
{
    public override SourceIterator<(A First, B Second)> GetIterator() => 
        new Zip2SourceIterator<A, B>(SourceA.GetIterator(), SourceB.GetIterator());
}

record Zip3Source<A, B, C>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC) : Source<(A First, B Second, C Third)>
{
    public override SourceIterator<(A First, B Second, C Third)> GetIterator() => 
        new Zip3SourceIterator<A, B, C>(SourceA.GetIterator(), SourceB.GetIterator(), SourceC.GetIterator());

}

record Zip4Source<A, B, C, D>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC, Source<D> SourceD) 
    : Source<(A First, B Second, C Third, D Fourth)>
{
    public override SourceIterator<(A First, B Second, C Third, D Fourth)> GetIterator() => 
        new Zip4SourceIterator<A, B, C, D>(SourceA.GetIterator(), SourceB.GetIterator(), SourceC.GetIterator(), SourceD.GetIterator());
}

record IteratorSyncSource<A>(IEnumerable<A> Items) : Source<A>
{
    public override SourceIterator<A> GetIterator() =>
        new IteratorSyncSourceIterator<A>{ Src = Items.GetIterator() };
}

record IteratorAsyncSource<A>(IAsyncEnumerable<A> Items) : Source<A>
{
    public override SourceIterator<A> GetIterator() =>
        new IteratorAsyncSourceIterator<A>{ Src = Items.GetIteratorAsync() };
}

record TransformSource<A, B>(Source<A> Source, Transducer<A, B> Transducer) : Source<B>
{
    public override SourceIterator<B> GetIterator() =>
        new TransformSourceIterator<A, B>(Source.GetIterator(), Transducer);
}

/*
record FoldWhileSource<S, A>(Schedule Schedule, Func<S, A, S> Folder, Func<(S State, A Value), bool> Pred, S State, Source<A> Src)
    : Source<S>
{
    public override SourceIterator<S> GetIterator() => 
        new FoldWhileSourceIterator<S,A>(Schedule, Folder, Pred, State, Src.GetIterator());
}

record FoldUntilSource<S, A>(Schedule Schedule, Func<S, A, S> Folder, Func<(S State, A Value), bool> Pred, S State, Source<A> Src)
    : Source<S>
{
    public override SourceIterator<S> GetIterator() => 
        new FoldUntilSourceIterator<S,A>(Schedule, Folder, Pred, State, Src.GetIterator());
}

record FilterSource<A>(Source<A> Source, Func<A, bool> Pred) : Source<A>
{
    public override SourceIterator<A> GetIterator() => 
        new FilterSourceIterator<A>(Source.GetIterator(), Pred);
}
*/
