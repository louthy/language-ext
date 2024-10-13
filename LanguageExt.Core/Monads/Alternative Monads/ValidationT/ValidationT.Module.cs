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
        ValidationT<F, M, A>.Lift(M.LiftIO(ma));
}

public partial class ValidationT
{
    public static ValidationT<L, M, A> Right<L, M, A>(A value)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Success(value);

    public static ValidationT<L, M, A> Left<L, M, A>(L value)  
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
        ValidationT<L, M, A>.Lift(M.LiftIO(ma));
    
    public static K<M, B> match<L, M, A, B>(ValidationT<L, M, A> ma, Func<A, B> Succ, Func<L, B> Fail) 
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.Match(Succ, Fail);
}
