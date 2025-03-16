using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSourceT<M, A>(SourceT<M, A> SourceTA, SourceT<M, A> SourceTB) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ChooseSourceTIterator<M, A>(SourceTA.GetIterator(), SourceTB.GetIterator());

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer)
    {
        try
        {
            return SourceTA.ReduceInternal(state, reducer).Choose(() => SourceTB.ReduceInternal(state, reducer));
        }
        catch
        {
            return SourceTB.ReduceInternal(state, reducer);
        }
    }
}
