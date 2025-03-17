using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip3Source<A, B, C>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC) : Source<(A First, B Second, C Third)>
{
    internal override SourceIterator<(A First, B Second, C Third)> GetIterator() =>
        new Zip3SourceIterator<A, B, C>(SourceA.GetIterator(), SourceB.GetIterator(), SourceC.GetIterator());
}
