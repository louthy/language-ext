using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public abstract record SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public abstract ReadResult<M, A> Read();
    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);

    public SourceTIterator<M, B> Map<B>(Func<A, B> f) =>
        new MapSourceTIterator<M, A, B>(this, f);

    public SourceTIterator<M, B> Bind<B>(Func<A, SourceTIterator<M, B>> f) =>
        new BindSourceTIterator<M, A, B>(this, f);

    public SourceTIterator<M, B> ApplyBack<B>(SourceTIterator<M, Func<A, B>> ff) =>
        new ApplySourceTIterator<M, A, B>(ff, this);

    public SourceTIterator<M, A> Choose(SourceTIterator<M, A> rhs) =>
        new ChooseSourceTIterator<M, A>(this, rhs);
}
