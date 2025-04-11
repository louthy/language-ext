using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSourceTIterator<M, A> : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal required Iterator<K<M, A>> Src;

    public override ReadResult<M, A> Read()
    {
        if (Src.IsEmpty) return ReadResult<M>.empty<A>();
        var head = Src.Head;
        Src = Src.Tail.Split();

        return ReadResult<M>.Value(head);
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(!token.IsCancellationRequested && !Src.IsEmpty);
}
