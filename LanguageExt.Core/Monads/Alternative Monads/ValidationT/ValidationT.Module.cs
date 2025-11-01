using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class ValidationT<F, M>
{
    public static ValidationT<F, M, A> Right<A>(A value) => 
        ValidationT<F, M, A>.Success(value);

    public static ValidationT<F, M, A> Left<A>(F value) => 
        ValidationT<F, M, A>.Fail(value);

    public static ValidationT<F, M, A> lift<A>(Validation<F, A> ma) => 
        ValidationT<F, M, A>.Lift(ma);

    public static ValidationT<F, M, A> lift<A>(Pure<A> ma) => 
        ValidationT<F, M, A>.Lift(ma);

    public static ValidationT<F, M, A> lift<A>(Fail<F> ma) => 
        ValidationT<F, M, A>.Lift(ma);

    public static ValidationT<F, M, A> liftIO<A>(IO<A> ma) =>  
        ValidationT<F, M, A>.Lift(M.LiftIOMaybe(ma));
}

public partial class ValidationT
{
    public static ValidationT<L, M, A> Success<L, M, A>(A value)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Success(value);

    public static ValidationT<L, M, A> Fail<L, M, A>(L value)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Fail(value);

    public static ValidationT<L, M, A> lift<L, M, A>(Validation<L, A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> lift<L, M, A>(K<M, A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> lift<L, M, A>(Pure<A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> lift<L, M, A>(Fail<L> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> liftIO<L, M, A>(IO<A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Lift(M.LiftIOMaybe(ma));
    
    public static K<M, B> match<F, M, A, B>(K<ValidationT<F, M>, A> ma, Func<F, B> Fail, Func<A, B> Succ) 
        where F : Monoid<F>
        where M : Monad<M> =>
        ma.Match(Fail, Succ);
}
