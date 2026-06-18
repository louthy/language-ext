using static LanguageExt.Prelude;

namespace LanguageExt;

public abstract partial class Iterator
{
    /// <summary>
    /// Repeat the iterator
    /// </summary>
    internal class OpRepeat<A>(Iterator<A> source, Iterator<Duration> repeats) : Iterator<A>
    {
        public Iterator<A> Source => 
            source;
        
        public Iterator<Duration> Repeats => 
            repeats;
        
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            new OpRepeatLive<A>(this, source, repeats).Next();
        
        public override string ToString() => 
            $"Repeat({source})";
    }
    
    /// <summary>
    /// Repeat the iterator
    /// </summary>
    internal class OpRepeatLive<A>(OpRepeat<A> source, Iterator<A> iter, Iterator<Duration> repeats) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            iter.Next() switch
            {
                (Exist<A>, _) head =>
                    head,

                _ => repeats.Next() switch
                     {
                         (Exist<Duration> (var dur), var tail) =>
                             YieldNext(dur, tail),

                         _ => Head.Nil<A>()
                     }
            };

        (Head<A> Head, Iterator<A> Tail) YieldNext(Duration dur, Iterator<Duration> tail)
        {
            ignore(IO.yieldFor(dur).Run());
            return new OpRepeatLive<A>(source, source.Source, tail).Next();
        }
        
        public override string ToString() => 
            $"Repeat({iter})";
    }
}
