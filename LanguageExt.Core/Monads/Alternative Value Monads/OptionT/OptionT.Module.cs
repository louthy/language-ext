using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class OptionT<M>
{
    public static OptionT<M, A> Some<A>(A value) => 
        OptionT<M, A>.Some(value);

    public static OptionT<M, A> None<A>() => 
        OptionT<M, A>.None;

    public static OptionT<M, A> lift<A>(Option<A> ma) => 
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> lift<A>(Pure<A> ma) => 
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> lift<A>(Fail<Unit> ma) => 
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> liftIO<A>(IO<A> ma) =>  
        OptionT<M, A>.Lift(M.LiftIO(ma));
}

public partial class OptionT
{
    public static OptionT<M, B> bind<M, A, B>(OptionT<M, A> ma, Func<A, OptionT<M, B>> f) 
        where M : Monad<M> =>
        ma.As().Bind(f);

    public static OptionT<M, B> map<M, A, B>(Func<A, B> f, OptionT<M, A> ma)  
        where M : Monad<M> =>
        ma.As().Map(f);

    public static OptionT<M, A> Some<M, A>(A value)  
        where M : Monad<M> =>
        OptionT<M, A>.Some(value);

    public static OptionT<M, A> None<M, A>()  
        where M : Monad<M> =>
        OptionT<M, A>.None;

    public static OptionT<M, B> apply<M, A, B>(OptionT<M, Func<A, B>> mf, OptionT<M, A> ma) 
        where M : Monad<M> =>
        mf.Apply(ma);

    public static OptionT<M, B> action<M, A, B>(OptionT<M, A> ma, OptionT<M, B> mb) 
        where M : Monad<M> =>
        ma.Action(mb);

    public static OptionT<M, A> lift<M, A>(Option<A> ma)  
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> lift<M, A>(K<M, A> ma)  
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> lift<M, A>(Pure<A> ma)  
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> lift<M, A>(Fail<Unit> ma)  
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma);

    public static OptionT<M, A> liftIO<M, A>(IO<A> ma)  
        where M : Monad<M> =>
        OptionT<M, A>.Lift(M.LiftIO(ma));
    
    public static K<M, B> match<M, A, B>(OptionT<M, A> ma, Func<A, B> Some, Func<B> None) 
        where M : Monad<M> =>
        ma.Match(Some, None);
}
