using System;
using System.Threading;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public static class ReadResult<M>
    where M : Monad<M>, Alternative<M>
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
    public abstract ReadResult<M, A> Choose(Func<ReadResult<M, A>> rhs);
}

record ReadM<M, A>(K<M, A> Value) : ReadResult<M, A>
    where M : Monad<M>, Alternative<M>
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
    
    public override ReadResult<M, A> Choose(Func<ReadResult<M, A>> rhs) =>
        rhs switch
        {
            ReadM<M, A> (var mr) =>
                ReadResult<M>.Value(Value.Choose(mr)),
            
            ReadIter<M, A> (var mr) => 
                ReadResult<M>.Iter(mr.Map(r => new LiftSourceTIterator<M, A>(Value).Choose(r)))
        };
}

record ReadIter<M, A>(K<M, SourceTIterator<M, A>> Iter) : ReadResult<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override ReadResult<M, B> Map<B>(Func<A, B> f) => 
        new ReadIter<M, B>(Iter.Map(iter => iter.Map(f)));

    public override ReadResult<M, B> ApplyBack<B>(ReadResult<M, Func<A, B>> ff) =>
        ff switch
        {
            ReadM<M, Func<A, B>> (var f) => 
                ReadResult<M>.Iter(Iter.Map(i => i.ApplyBack(new LiftSourceTIterator<M, Func<A, B>>(f)))),
            
            ReadIter<M, Func<A, B>> (var mf) => 
                ReadResult<M>.Iter(Iter.Bind(i => mf.Map(i.ApplyBack)))
        };    
    
    public override ReadResult<M, A> Choose(Func<ReadResult<M, A>> rhs) =>
        rhs switch
        {
            ReadM<M, A> (var mr) =>
                ReadResult<M>.Iter(Iter.Map(l => l.Choose(new LiftSourceTIterator<M, A>(mr)))),
            
            ReadIter<M, A> (var mr) => 
                ReadResult<M>.Iter(Iter.Bind(l => mr.Map(l.Choose)))
        };
}

