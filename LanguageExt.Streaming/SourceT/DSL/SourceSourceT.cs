using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record SourceSourceT<M, A>(Source<K<M, A>> Source) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        M.LiftIO(Source.Reduce(M.Pure(state), (ms, ma) => ms.Bind(s => reducer(s, ma)))).Flatten();
}
