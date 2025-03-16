using LanguageExt.Traits;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceT<M, A>(Seq<SourceT<M, A>> SourceTs) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new CombineSourceTIterator<M, A>(SourceTs.Map(x => x.GetIterator()));
}
