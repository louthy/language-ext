using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class ValidationT
{
    public static ValidationT<L, M, A> Success<L, M, A>(A value)
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => M.Pure(Validation.Success<L, A>(value)));

    public static ValidationT<L, M, A> Fail<L, M, A>(L value)  
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => M.Pure(Validation.Fail<L, A>(value)));

    public static ValidationT<L, M, A> lift<L, M, A>(Validation<L, A> ma)
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => M.Pure(ma));

    public static ValidationT<L, M, A> lift<L, M, A>(K<M, A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => ma.Map(Validation.Success<L, A>));

    public static ValidationT<L, M, A> lift<L, M, A>(Pure<A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => M.Pure(Validation.Success<L, A>(ma.Value)));

    public static ValidationT<L, M, A> lift<L, M, A>(Fail<L> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => M.Pure(Validation.Fail<L, A>(ma.Value)));

    public static ValidationT<L, M, A> liftIO<L, M, A>(IO<A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        new (_ => M.LiftIOMaybe(ma).Map(Validation.Success<L, A>));
    
    public static K<M, B> match<F, M, A, B>(K<ValidationT<F, M>, A> ma, Func<F, B> Fail, Func<A, B> Succ) 
        where F : Monoid<F>
        where M : Monad<M> =>
        ma.Match(Fail, Succ);

    internal static ValidationT<L, M, A> liftI<L, M, A>(Validation<L, A> ma)
        where M : Monad<M> =>
        new (_ => M.Pure(ma));

    internal static ValidationT<L, M, A> liftI<L, M, A>(K<M, A> ma)  
        where M : Monad<M> =>
        new (_ => ma.Map(Validation.SuccessI<L, A>));

    internal static ValidationT<L, M, A> liftIOI<L, M, A>(IO<A> ma)  
        where M : Monad<M> =>
        new (_ => M.LiftIOMaybe(ma).Map(Validation.SuccessI<L, A>));
    
    internal static ValidationT<L, M, A> SuccessI<L, M, A>(A value)  
        where M : Monad<M> =>
        new (_ => M.Pure(Validation.SuccessI<L, A>(value)));

    internal static ValidationT<L, M, A> FailI<L, M, A>(L value)  
        where M : Monad<M> =>
        new (_ => M.Pure(Validation.FailI<L, A>(value)));
}
