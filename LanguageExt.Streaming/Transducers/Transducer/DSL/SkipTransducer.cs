namespace LanguageExt;

record SkipTransducer<A>(int Amount) : Transducer<A, A> 
{
    public override ReducerIO<A, S> Reduce<S>(ReducerIO<A, S> reducer)
    {
        var amount = Amount;
        return (s, x) =>
                   IO.liftVAsync(async e =>
                                 {
                                     if(e.Token.IsCancellationRequested) return Reduced.Done(s);
                                     if (amount > 0)
                                     {
                                         amount--;
                                         return Reduced.Continue(s);
                                     }
                                     else
                                     {
                                         return await reducer(s, x).RunAsync(e);
                                     }
                                 });
    }
    
    public override TransducerM<M, A, A> Lift<M>() => 
        new SkipTransducerM<M, A>(Amount);
}
