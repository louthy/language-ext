using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class FinT<M>
{
    public static FinT<M, A> Succ<A>(A value) => 
        FinT<M, A>.Succ(value);

    public static FinT<M, A> Fail<A>(Error value) => 
        FinT<M, A>.Fail(value);

    public static FinT<M, A> lift<A>(Fin<A> ma) => 
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> lift<A>(Pure<A> ma) => 
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> lift<A>(Fail<Error> ma) => 
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> liftIO<A>(IO<A> ma) =>  
        FinT<M, A>.Lift(M.LiftIO(ma));
}

public partial class FinT
{
    public static FinT<M, B> bind<M, A, B>(FinT<M, A> ma, Func<A, FinT<M, B>> f) 
        where M : Monad<M> =>
        ma.As().Bind(f);

    public static FinT<M, B> map<M, A, B>(Func<A, B> f, FinT<M, A> ma)  
        where M : Monad<M> =>
        ma.As().Map(f);

    public static FinT<M, A> Succ<M, A>(A value)  
        where M : Monad<M> =>
        FinT<M, A>.Succ(value);

    public static FinT<M, A> Fail<M, A>(Error value)  
        where M : Monad<M> =>
        FinT<M, A>.Fail(value);

    public static FinT<M, B> apply<M, A, B>(FinT<M, Func<A, B>> mf, FinT<M, A> ma) 
        where M : Monad<M> =>
        mf.Apply(ma);

    public static FinT<M, B> action<M, A, B>(FinT<M, A> ma, FinT<M, B> mb) 
        where M : Monad<M> =>
        ma.Action(mb);

    public static FinT<M, A> lift<M, A>(Fin<A> ma)  
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> lift<M, A>(Either<Error, A> ma)  
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> lift<M, A>(K<M, A> ma)  
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> lift<M, A>(Pure<A> ma)  
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> lift<M, A>(Fail<Error> ma)  
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma);

    public static FinT<M, A> liftIO<M, A>(IO<A> ma)  
        where M : Monad<M> =>
        new (M.LiftIO(ma.Try().runFin));
    
    public static K<M, B> match<M, A, B>(FinT<M, A> ma, Func<A, B> Succ, Func<Error, B> Fail) 
        where M : Monad<M> =>
        ma.Match(Succ, Fail);
}
