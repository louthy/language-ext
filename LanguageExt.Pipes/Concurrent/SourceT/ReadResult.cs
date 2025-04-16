using System;
using System.Threading;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public static class ReadResult<M>
    where M : MonadIO<M>, Alternative<M>
{
    public static readonly ReadResult<M, CancellationToken> token =
        Value(M.LiftIO(IO.token));
    
    public static ReadResult<M, A> empty<A>() =>
        Value(M.Empty<A>());
    
    public static ReadResult<M, A> Value<A>(K<M, A> ma) =>
        new ReadM<M, A>(ma);
    
    public static ReadResult<M, A> Iter<A>(K<M, SourceTIterator<M, A>> iter) =>
        new ReadIter<M, A>(iter);    
}

public abstract record ReadResult<M, A>
{
    public abstract ReadResult<M, B> Map<B>(Func<A, B> f);
    public abstract ReadResult<M, B> ApplyBack<B>(ReadResult<M, Func<A, B>> ff);
    public abstract ReadResult<M, (A First, B Second)> Zip<B>(ReadResult<M, B> second);
    
    public ReadResult<M, (A First, B Second, C Third)> Zip<B, C>(ReadResult<M, B> second, ReadResult<M, C> third) =>
        Zip(second).Zip(third).Map<(A First, B Second, C Third)>(
            pp => (First: pp.First.First, Second: pp.First.Second, Third: pp.Second));
    
    public ReadResult<M, (A First, B Second, C Third, D Fourth)> Zip<B, C, D>(
        ReadResult<M, B> second, 
        ReadResult<M, C> third, 
        ReadResult<M, D> fourth) =>
        Zip(second).Zip(third.Zip(fourth)).Map<(A First, B Second, C Third, D Fourth)>(
            pp => (pp.First.First, pp.First.Second, pp.Second.First, pp.Second.Second));
}

record ReadM<M, A>(K<M, A> Value) : ReadResult<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, B> Map<B>(Func<A, B> f) => 
        new ReadM<M, B>(Value.Map(f));

    public override ReadResult<M, B> ApplyBack<B>(ReadResult<M, Func<A, B>> ff) =>
        ff switch
        {
            ReadM<M, Func<A, B>> (var f) => 
                ReadResult<M>.Value(f.Apply(Value)),
            
            ReadIter<M, Func<A, B>> (var mf) => 
                ReadResult<M>.Iter(mf.Map(f => new LiftSourceTIterator<M, A>(Value).ApplyBack(f)))
        };
    
    public override ReadResult<M, (A First, B Second)> Zip<B>(ReadResult<M, B> second) =>
        second switch
        {
            ReadM<M, B> (var m2) =>
                ReadResult<M>.Value(Value.Zip(m2)),
            
            ReadIter<M, B> (var mr) => 
                ReadResult<M>.Iter(mr.Map(r => new LiftSourceTIterator<M, A>(Value).Zip(r)))
        };
}

record ReadIter<M, A>(K<M, SourceTIterator<M, A>> Iter) : ReadResult<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, B> Map<B>(Func<A, B> f) =>
        new ReadIter<M, B>(Iter.Map(iter => iter.Map(f)));

    public override ReadResult<M, B> ApplyBack<B>(ReadResult<M, Func<A, B>> ff) =>
        ff switch
        {
            ReadM<M, Func<A, B>> (var mf) =>
                ReadResult<M>.Iter(Iter.Map(mx => mx.ApplyBack(new LiftSourceTIterator<M, Func<A, B>>(mf)))),

            ReadIter<M, Func<A, B>> (var mf) =>
                ReadResult<M>.Iter(Iter.Bind(mx => mf.Map(mx.ApplyBack)))
        };

    public override ReadResult<M, (A First, B Second)> Zip<B>(ReadResult<M, B> second) =>
        second switch
        {
            ReadM<M, B> (var my) =>
                ReadResult<M>.Iter(Iter.Map(mx => mx.Zip(new LiftSourceTIterator<M, B>(my)))),

            ReadIter<M, B> (var my) =>
                ReadResult<M>.Iter(Iter.Bind(mx => my.Map(mx.Zip)))
        };
}

