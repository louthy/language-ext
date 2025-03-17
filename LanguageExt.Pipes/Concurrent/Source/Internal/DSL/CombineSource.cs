using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record CombineSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new CombineSourceIterator<A>(Sources.Map(x => x.GetIterator()));
}
