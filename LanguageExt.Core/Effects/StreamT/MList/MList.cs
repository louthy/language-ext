using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract record MList<A> : K<MList, A>
{
    public abstract MList<B> Map<B>(Func<A, B> f);

    public static MList<A> Nil = 
        MNil<A>.Default;
    
    public static MList<A> Cons<M>(A head, K<M, MList<A>> tail) 
        where M : Monad<M> => 
        new MCons<M, A>(head, tail);
    
    internal static MList<A> Iter<M>(A head, IEnumerator<A> tail) 
        where M : Monad<M> => 
        new MIter<M, A>(head, tail);

    public K<M, MList<A>> Append<M>(K<M, MList<A>> ys)
        where M : Monad<M> =>
        this switch
        {
            MNil<A>                     => ys,
            MCons<M, A> (var h, var t)  => M.Pure(Cons(h, t.Append(ys))),
            MIter<M, A> (var h, _) iter => M.Pure(Cons(h, iter.TailM().Append(ys))),
            _                           => throw new NotSupportedException()
        };
}

public record MNil<A> : MList<A>
{
    public static readonly MList<A> Default = new MNil<A>();

    public override MList<B> Map<B>(Func<A, B> f) =>
        MNil<B>.Default;
}

public record MCons<M, A>(A Head, K<M, MList<A>> Tail) : MList<A>
    where M : Monad<M>
{
    public override MList<B> Map<B>(Func<A, B> f) => 
        new MCons<M, B>(f(Head), Tail.Map(l => l.Map(f)));
}

public record MIter<M, A>(A Head, IEnumerator<A> Tail) : MList<A>
    where M : Monad<M>
{
    public override MList<B> Map<B>(Func<A, B> f) => 
        new MIter<M, B>(f(Head), new MListMapEnumerator<A, B>(Tail, f));

    public MList<A> ToCons() =>
        new MCons<M, A>(Head, StreamEnumerableT<M, A>.Lift(Tail).runListT);

    public K<M, MList<A>> TailM() =>
        StreamEnumerableT<M, A>.Lift(Tail).runListT;
}

class MListMapEnumerator<A, B>(IEnumerator<A> Iter, Func<A, B> Map) : IEnumerator<B>
{
    object IEnumerator.Current => 
        Current!;

    public B Current =>
        Map(Iter.Current);

    public bool MoveNext() =>
        Iter.MoveNext();

    public void Reset() 
    { }

    public void Dispose()
    { }
}
