using System;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public partial class ValidationT<L, M>
{
    public static ValidationT<L, M, A> Right<A>(A value) => 
        ValidationT<L, M, A>.Success(value);

    public static ValidationT<L, M, A> Left<A>(L value) => 
        ValidationT<L, M, A>.Fail(value);

    public static ValidationT<L, M, A> lift<A>(Either<L, A> ma) => 
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> lift<A>(Pure<A> ma) => 
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> lift<A>(Fail<L> ma) => 
        ValidationT<L, M, A>.Lift(ma);

    public static ValidationT<L, M, A> liftIO<A>(IO<A> ma) =>  
        ValidationT<L, M, A>.Lift(M.LiftIO(ma));
}

public partial class ValidationT
{
    public static ValidationT<L, M, B> bind<L, M, A, B>(ValidationT<L, M, A> ma, Func<A, ValidationT<L, M, B>> f)
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.As().Bind(f);

    public static ValidationT<L, M, B> map<L, M, A, B>(Func<A, B> f, ValidationT<L, M, A> ma)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.As().Map(f);

    public static ValidationT<L, M, A> Right<L, M, A>(A value)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Success(value);

    public static ValidationT<L, M, A> Left<L, M, A>(L value)  
        where L : Monoid<L>
        where M : Monad<M> =>
        ValidationT<L, M, A>.Fail(value);

    public static ValidationT<L, M, B> apply<L, M, A, B>(ValidationT<L, M, Func<A, B>> mf, ValidationT<L, M, A> ma) 
        where L : Monoid<L>
        where M : Monad<M> =>
        mf.Apply(ma);

    public static ValidationT<L, M, B> action<L, M, A, B>(ValidationT<L, M, A> ma, ValidationT<L, M, B> mb) 
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.Action(mb);

    public static ValidationT<L, M, A> lift<L, M, A>(Either<L, A> ma)  
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
    
    public static K<M, B> match<L, M, A, B>(ValidationT<L, M, A> ma, Func<L, B> Left, Func<A, B> Right) 
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.Match(Left, Right);
}
