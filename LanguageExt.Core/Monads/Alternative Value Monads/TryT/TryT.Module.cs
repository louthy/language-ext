using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class TryT<M>
{
    public static TryT<M, A> Succ<A>(A value) => 
        TryT<M, A>.Succ(value);

    public static TryT<M, A> Fail<A>(Error value) => 
        TryT<M, A>.Fail(value);

    public static TryT<M, A> lift<A>(Func<Fin<A>> ma) => 
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<A>(Fin<A> ma) => 
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<A>(Pure<A> ma) => 
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<A>(Fail<Error> ma) => 
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<A>(Error ma) => 
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> liftIO<A>(IO<A> ma) =>  
        TryT<M, A>.Lift(M.LiftIO(ma));
}

public class TryT
{
    public static TryT<M, B> bind<M, A, B>(TryT<M, A> ma, Func<A, TryT<M, B>> f) 
        where M : Monad<M> =>
        ma.As().Bind(f);

    public static TryT<M, B> map<M, A, B>(Func<A, B> f, TryT<M, A> ma)  
        where M : Monad<M> =>
        ma.As().Map(f);

    public static TryT<M, A> Succ<M, A>(A value)  
        where M : Monad<M> =>
        TryT<M, A>.Succ(value);

    public static TryT<M, A> Fail<M, A>(Error value)  
        where M : Monad<M> =>
        TryT<M, A>.Fail(value);

    public static TryT<M, B> apply<M, A, B>(TryT<M, Func<A, B>> mf, TryT<M, A> ma) 
        where M : Monad<M> =>
        mf.Apply(ma);

    public static TryT<M, B> action<M, A, B>(TryT<M, A> ma, TryT<M, B> mb) 
        where M : Monad<M> =>
        ma.Action(mb);

    public static TryT<M, A> lift<M, A>(Func<Fin<A>> ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<M, A>(Fin<A> ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<M, A>(K<M, A> ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<M, A>(Pure<A> ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<M, A>(Fail<Error> ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> lift<M, A>(Error ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma);

    public static TryT<M, A> liftIO<M, A>(IO<A> ma)  
        where M : Monad<M> =>
        TryT<M, A>.Lift(M.LiftIO(ma));
    
    public static K<M, B> match<M, A, B>(TryT<M, A> ma, Func<A, B> Succ, Func<Error, B> Fail) 
        where M : Monad<M> =>
        ma.Match(Succ, Fail);
}
