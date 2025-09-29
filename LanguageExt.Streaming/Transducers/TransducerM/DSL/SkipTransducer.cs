using LanguageExt.Traits;

namespace LanguageExt;

record SkipTransducerM<M, A>(int Amount) : TransducerM<M, A, A>
    where M : Applicative<M>
{
    int amount = Amount;
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, A, S> reducer)  
    {
        return (s, x) =>
               {
                   if (amount > 0)
                   {
                       amount--;
                       return M.Pure(s);
                   }
                   else
                   {
                       return reducer(s, x);
                   }
               };
    }
}
