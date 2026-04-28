using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.Traits.Domain;

public static partial class RuleT
{
    public class All<R1, R2, T, M, A>
        : RuleT<All<R1, R2, T, M, A>, T, M, A>
        where R1 : RuleT<R1, T, M, A>, new()
        where R2 : RuleT<R2, T, M, A>, new()
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        public R1 First => R1.Instance;
        
        public R2 Second => R2.Instance;
        
        public static K<T, bool> Check(K<M, A> value) => 
            from v in T.Lift(value)
            from r1Val in R1.Check(M.Pure(v))
            from r2Val in R2.Check(M.Pure(v))
            select r1Val && r2Val;

    }

    public class Any<R1, R2, T, M, A>
        : RuleT<Any<R1, R2, T, M, A>, T, M, A>
        where R1 : RuleT<R1, T, M, A>, new()
        where R2 : RuleT<R2, T, M, A>, new()
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        public R1 First => R1.Instance;
        
        public R2 Second => R2.Instance;
        
        public static K<T, bool> Check(K<M, A> value) => 
            from v in T.Lift(value)
            from r1Val in R1.Check(M.Pure(v))
            from r2Val in R2.Check(M.Pure(v))
            select r1Val || r2Val;
    }

    public class Not<R, T, M, A> : RuleT<Not<R, T, M, A>, T, M, A>
        where R : RuleT<R, T, M, A>, new()
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        public R NegatedRule => R.Instance;
        
        public static K<T, bool> Check(K<M, A> value) => 
            from rVal in R.Check(value)
            select !rVal;
    }

    public class Lift<R, T, M, A> : RuleT<Lift<R, T, M, A>, T, M, A>
        where R : RuleK<R, M, A>, new()
        where T : MonadT<T, M>
        where M : Monad<M>
    {
        public R Lifted => R.Instance;
        
        public static K<T, bool> Check(K<M, A> value) => 
            T.Pure(R.Check(value));
    }
}
