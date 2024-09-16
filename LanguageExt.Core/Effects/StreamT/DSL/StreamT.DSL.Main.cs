using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamMainT<M, A>(K<M, MList<A>> runListT) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = runListT;

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamMainT<M, B>(runListT.Map(la => la.Map(f)));

    public override StreamT<M, A> Tail() =>
        new StreamMainT<M, A>(
            from ml in runListT
            from rl in ml switch
                       {
                           MNil<A> => 
                               Empty.runListT,

                           MCons<M, A>(_, var t) =>
                               new StreamMainT<M, A>(t).runListT,

                           MIter<M, A> iter  =>
                               iter.TailM(),
                           
                           _ => throw new NotSupportedException()
                       }
            select rl);

}
