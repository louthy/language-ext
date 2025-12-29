namespace LanguageExt;

record TakeTransducer<A>(int Amount) : Transducer<A, A> 
{
    public override ReducerIO<A, S> Reduce<S>(ReducerIO<A, S> reducer)
    {
        var taken = 0;
        return (s, x) => 
                   IO.liftVAsync(async e =>
                                 {
                                     if(e.Token.IsCancellationRequested) return Reduced.Done(s);
                                     if (taken < Amount)
                                     {
                                         taken++;
                                         return await reducer(s, x).RunAsync(e);
                                     }
                                     else
                                     {
                                         return Reduced.Done(s);
                                     }
                                 });
    }
}
