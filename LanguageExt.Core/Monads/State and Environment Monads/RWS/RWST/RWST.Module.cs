using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class RWST
{
    public static RWST<R, W, S, M, A> pure<R, W, S, M, A>(A value)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        RWST<R, W, S, M, A>.Pure(value);

    public static RWST<R, W, S, M, A> lift<R, W, S, M, A>(K<M, A> ma)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        RWST<R, W, S, M, A>.Lift(ma);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`ReaderT`</returns>
    public static RWST<R, W, S, M, A> liftIO<R, W, S, M, A>(IO<A> effect)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        RWST<R, W, S, M, A>.LiftIO(effect);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Alternative behaviours
    //
    
    public static RWST<R, W, S, M, A> combine<R, W, S, M, A>(
        K<RWST<R, W, S, M>, A> ma,
        K<RWST<R, W, S, M>, A> mb)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        SemigroupK.combine(ma, mb).As();
    
    public static RWST<R, W, S, M, A> combine<R, W, S, M, A>(
        K<IO, A> ma,
        K<RWST<R, W, S, M>, A> mb)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        SemigroupK.combine(RWST<R, W, S, M, A>.LiftIO(ma), mb).As();
    
    public static RWST<R, W, S, M, A> combine<R, W, S, M, A>(
        K<RWST<R, W, S, M>, A> ma,
        K<IO, A> mb)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        SemigroupK.combine(ma, RWST<R, W, S, M, A>.LiftIO(mb)).As();
    
    public static RWST<R, W, S, M, A> combine<R, W, S, M, A>(
        Ask<R, A> ma,
        K<RWST<R, W, S, M>, A> mb)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        SemigroupK.combine(asks<R, W, S, M, A>(ma.F), mb).As();
    
    public static RWST<R, W, S, M, A> combine<R, W, S, M, A>(
        K<RWST<R, W, S, M>, A> ma,
        Ask<R, A> mb)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        SemigroupK.combine(ma, asks<R, W, S, M, A>(mb.F)).As();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Reader behaviours
    //
    
    public static RWST<R, W, S, M, R> ask<R, W, S, M>() 
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Readable.ask<RWST<R, W, S, M>, R>().As();

    public static RWST<R, W, S, M, A> asks<R, W, S, M, A>(Func<R, A> f)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Readable.asks<RWST<R, W, S, M>, R, A>(f).As();

    public static RWST<R, W, S, M, A> asksM<R, W, S, M, A>(Func<R, K<M, A>> f)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        RWST<R, W, S, M, A>.AsksM(f);

    public static RWST<R, W, S, M, A> asksM<R, W, S, M, A>(Func<R, K<RWST<R, W, S, M>, A>> f)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        Readable.asksM(f).As();

    public static RWST<R, W, S, M, A> local<R, W, S, M, A>(Func<R, R> f, K<RWST<R, W, S, M>, A> ma)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        Readable.local(f, ma).As();

    public static RWST<R, W, S, M, A> with<R, R1, W, S, M, A>(Func<R, R1> f, K<RWST<R1, W, S, M>, A> ma) 
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        ma.As().With(f);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Writer behaviours
    //

    /// <summary>
    /// Tell is an action that produces the writer output
    /// </summary>
    /// <param name="item">Item to tell</param>
    /// <typeparam name="W">Writer type</typeparam>
    /// <returns>Structure with the told item</returns>
    public static RWST<R, W, S, M, Unit> tell<R, W, S, M>(W item)
        where M : Monad<M>, SemigroupK<M>
        where W : Monoid<W> =>
        Writable.tell<RWST<R, W, S, M>, W>(item).As();

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static RWST<R, W, S, M, A> write<R, W, S, M, A>((A, W) item)
        where M : Monad<M>, SemigroupK<M> 
        where W : Monoid<W> =>
        Writable.write<W, RWST<R, W, S, M>, A>(item).As();

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static RWST<R, W, S, M, A> write<R, W, S, M, A>(A value, W item)
        where M : Monad<M>, SemigroupK<M> 
        where W : Monoid<W> =>
        Writable.write<W, RWST<R, W, S, M>, A>(value, item).As();

    /// <summary>
    /// `pass` is an action that executes the `action`, which returns a value and a
    /// function; it then returns the value with the output having been applied to
    /// the function.
    /// </summary>
    public static RWST<R, W, S, M, A> pass<R, W, S, M, A>(RWST<R, W, S, M, (A Value, Func<W, W> Function)> action)
        where M : Monad<M>, SemigroupK<M> 
        where W : Monoid<W> =>
        Writable.pass(action).As();

    /// <summary>
    /// `listen` executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static RWST<R, W, S, M, (A Value, W Output)> listen<R, W, S, M, A>(RWST<R, W, S, M, A> ma)
        where M : Monad<M>, SemigroupK<M>
        where W : Monoid<W> =>
        Writable.listen<W, RWST<R, W, S, M>, A>(ma).As();

    /// <summary>
    /// `listens` executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static RWST<R, W, S, M, (A Value, B Output)> listens<R, W, S, M, A, B>(Func<W, B> f, RWST<R, W, S, M, A> ma)
        where M : Monad<M>, SemigroupK<M>
        where W : Monoid<W> =>
        Writable.listens(f, ma).As();

    /// <summary>
    /// `censor` executes the action `ma` and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public static RWST<R, W, S, M, A> censor<R, W, S, M, A>(Func<W, W> f, RWST<R, W, S, M, A> ma)
        where M : Monad<M>, SemigroupK<M> 
        where W : Monoid<W> =>
        Writable.censor(f, ma).As();    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  State behaviours
    //
    
    public static RWST<R, W, S, M, S> get<R, W, S, M>()
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Stateful.get<RWST<R, W, S, M>, S>().As();
    
    public static RWST<R, W, S, M, A> gets<R, W, S, M, A>(Func<S, A> f) 
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Stateful.gets<RWST<R, W, S, M>, S, A>(f).As();

    public static RWST<R, W, S, M, A> getsM<R, W, S, M, A>(Func<S, K<M, A>> f) 
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        RWST<R, W, S, M, A>.GetsM(f);

    public static RWST<R, W, S, M, A> getsM<R, W, S, M, A>(Func<S, K<RWST<R, W, S, M>, A>> f) 
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Stateful.getsM(f).As();

    public static RWST<R, W, S, M, Unit> put<R, W, S, M>(S state)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Stateful.put<RWST<R, W, S, M>, S>(state).As();

    public static RWST<R, W, S, M, Unit> modify<R, W, S, M>(Func<S, S> f)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> => 
        Stateful.modify<RWST<R, W, S, M>, S>(f).As();

    public static RWST<R, W, S, M, Unit> modifyM<R, W, S, M>(Func<S, K<M, S>> f)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        RWST<R, W, S, M, Unit>.ModifyM(f);

    public static RWST<R, W, S, M, Unit> modifyM<R, W, S, M>(Func<S, K<RWST<R, W, S, M>, S>> f)  
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        Stateful.modifyM(f).As();
}
