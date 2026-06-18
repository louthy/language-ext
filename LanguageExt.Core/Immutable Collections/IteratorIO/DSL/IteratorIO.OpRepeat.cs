namespace LanguageExt;

public abstract partial class IteratorIO
{
    /// <summary>
    /// Repeat the iterator
    /// </summary>
    internal class OpRepeat<A>(IteratorIO<A> source, Iterator<Duration> repeats) : IteratorIO<A>
    {
        public IteratorIO<A> Source => 
            source;
        
        public Iterator<Duration> Repeats => 
            repeats;
        
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            new OpRepeatLive<A>(this, source, repeats).NextIO();
        
        public override IteratorIO<A> Using() => 
            new OpRepeat<A>(source.Using(), repeats);
        
        public override string ToString() => 
            $"Repeat({source})";
    }
    
    /// <summary>
    /// Repeat the iterator
    /// </summary>
    internal class OpRepeatLive<A>(OpRepeat<A> source, IteratorIO<A> iter, Iterator<Duration> repeats) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() >>
            (head => head switch
                     {
                         (Exist<A>, _) =>
                             IO.pure(head),

                         _ => repeats.Next() switch
                              {
                                  (Exist<Duration> (var dur), var tail) =>
                                      IO.yieldFor(dur) >> new OpRepeatLive<A>(source, source.Source, tail).NextIO(),

                                  _ => IO.pure(Head.NilIO<A>())
                              }
                     });
        
        public override IteratorIO<A> Using() => 
            new OpRepeatLive<A>(source, iter.Using(), repeats);
        
        public override string ToString() => 
            $"Repeat({iter})";
    }
}
