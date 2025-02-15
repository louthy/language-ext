using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record SourcePure<A>(A Value) : Source<A>
{
    public override Source<B> Map<B>(Func<A, B> f) => 
        new SourceMap<A, B>(this, f);

    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        new SourceBind<A, B>(this, f);

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) => 
        new SourceApply<A, B>(this, ff);

    public override IO<A> Read() =>
        IO.pure(Value);
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(true);
}

record SourceEmpty<A> : Source<A>
{
    public static readonly Source<A> Default = new SourceEmpty<A>();
    
    public override Source<B> Map<B>(Func<A, B> f) => 
        new SourceMap<A, B>(this, f);

    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        new SourceBind<A, B>(this, f);

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) => 
        new SourceApply<A, B>(this, ff);

    public override IO<A> Read() =>
        IO.fail<A>(Errors.SourceClosed);
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}

record SourceReader<A>(ChannelReader<A> Reader, string Label) : Source<A>
{
    public override Source<B> Map<B>(Func<A, B> f) => 
        new SourceMap<A, B>(this, f);

    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        new SourceBind<A, B>(this, f);

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) => 
        new SourceApply<A, B>(this, ff);

    public override IO<A> Read() =>
        IO.liftVAsync(e => Reader.WaitToReadAsync(e.Token))
          .Bind(f => f ? IO.liftVAsync(e => Reader.ReadAsync(e.Token))
                       : IO.fail<A>(Errors.SourceClosed));
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Reader.WaitToReadAsync(token);
}

record SourceMap<A, B>(Source<A> Source, Func<A, B> F) : Source<B>
{
    public override Source<C> Map<C>(Func<B, C> f) =>
        new SourceMap<A, C>(Source, x => f(F(x)));

    public override Source<C> Bind<C>(Func<B, Source<C>> f) => 
        new SourceBind<A, C>(Source, x => f(F(x)));

    public override Source<C> ApplyBack<C>(Source<Func<B, C>> ff) => 
        new SourceApply<B, C>(this, ff);

    public override IO<B> Read() =>
        Source.Read().Map(F);
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Source.ReadyToRead(token);
}

record SourceApply<A, B>(Source<A> Source, Source<Func<A, B>> FF) : Source<B>
{
    public override Source<C> Map<C>(Func<B, C> f) =>
        new SourceMap<B, C>(this, f);

    public override Source<C> Bind<C>(Func<B, Source<C>> f) => 
        new SourceBind<B, C>(this, f);

    public override Source<C> ApplyBack<C>(Source<Func<B, C>> ff) => 
        new SourceApply<B, C>(this, ff);

    public override IO<B> Read() =>
        FF.Read().Apply(Source.Read());
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Source.ReadyToRead(token);
}

record SourceBind<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    public override Source<C> Map<C>(Func<B, C> f) =>
        new SourceMap<B, C>(this, f);

    public override Source<C> Bind<C>(Func<B, Source<C>> f) =>
        new SourceBind<B, C>(this, f);

    public override Source<C> ApplyBack<C>(Source<Func<B, C>> ff) =>
        new SourceApply<B, C>(this, ff);

    public override IO<B> Read() =>
        Source.Read().Bind(x => F(x).Read());

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Source.ReadyToRead(token);
}

record SourceCombine<A>(Seq<Source<A>> Sources) : Source<A>
{
    public override Source<B> Map<B>(Func<A, B> f) =>
        new SourceMap<A, B>(this, f);

    public override Source<B> Bind<B>(Func<A, Source<B>> f) =>
        new SourceBind<A, B>(this, f);

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        new SourceApply<A, B>(this, ff);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceInternal.ReadyToRead(Sources, token);

    public override IO<A> Read() =>
        IO.liftVAsync<A>(e => SourceInternal.Read(Sources, e));
}

record SourceChoose<A>(Source<A> Left, Source<A> Right) : Source<A>
{
    public override Source<B> Map<B>(Func<A, B> f) =>
        new SourceMap<A, B>(this, f);

    public override Source<B> Bind<B>(Func<A, Source<B>> f) =>
        new SourceBind<A, B>(this, f);

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        new SourceApply<A, B>(this, ff);

    public override IO<A> Read() =>
        Left.Read() | Right.Read();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceInternal.ReadyToRead([Left, Right], token);
}
