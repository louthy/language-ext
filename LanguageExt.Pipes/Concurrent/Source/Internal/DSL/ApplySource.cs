using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes.Concurrent;

record ApplySource<A, B>(Source<Func<A, B>> FF, Source<A> FA) : Source<B>
{
    internal override SourceIterator<B> GetIterator() =>
        new ApplySourceIterator<A, B>(FF.GetIterator(), FA.GetIterator());
}
