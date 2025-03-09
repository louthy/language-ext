namespace LanguageExt;

record TakeTransducer<A>(int Amount) : Transducer<A, A> 
{
    public override Reducer<A, S> Reduce<S>(Reducer<A, S> reducer)
    {
        var taken = 0;
        return (s, x) =>
               {
                   if (taken < Amount)
                   {
                       taken++;
                       return reducer(s, x);
                   }
                   else
                   {
                       return Reduced.DoneAsync(s);
                   }
               };
    }
}
