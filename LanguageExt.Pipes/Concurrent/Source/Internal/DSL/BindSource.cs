using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record BindSource<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    internal override SourceIterator<B> GetIterator() =>
        new BindSourceIterator<A, B>(Source.GetIterator(), x => F(x).GetIterator());
}
