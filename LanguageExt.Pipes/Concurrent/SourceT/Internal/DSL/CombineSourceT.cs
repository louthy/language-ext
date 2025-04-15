using LanguageExt.Traits;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new CombineSourceTIterator<M, A>(Sources.Map(x => x.GetIterator()));
}
