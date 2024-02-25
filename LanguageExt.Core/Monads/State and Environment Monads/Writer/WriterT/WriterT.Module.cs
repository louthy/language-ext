using System;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;


/// <summary>
/// `MonadWriterT` trait implementation for `WriterT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class WriterT<W>
    where W : Monoid<W>
{
    public static WriterT<W, M, A> lift<M, A>(K<M, A> ma)
        where M : Monad<M>, SemiAlternative<M> => 
        WriterT<W, M, A>.Lift(ma);
}

/// <summary>
/// `MonadWriterT` trait implementation for `WriterT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public partial class WriterT<W, M>
{
    public static WriterT<W, M, A> Pure<A>(A value) => 
        WriterT<W, M, A>.Pure(value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> liftIO<A>(IO<A> effect) =>
        WriterT<W, M, A>.LiftIO(effect);
}

/// <summary>
/// `MonadWriterT` trait implementation for `WriterT` 
/// </summary>
/// <typeparam name="S">State environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
public class WriterT
{
    public static WriterT<W, M, B> bind<W, M, A, B>(WriterT<W, M, A> ma, Func<A, WriterT<W, M, B>> f) 
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> => 
        ma.As().Bind(f);

    public static WriterT<W, M, B> map<W, M, A, B>(Func<A, B> f, WriterT<W, M, A> ma)  
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> => 
        ma.As().Map(f);

    public static WriterT<W, M, A> Pure<W, M, A>(A value)  
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> => 
        WriterT<W, M, A>.Pure(value);

    public static WriterT<W, M, B> apply<W, M, A, B>(WriterT<W, M, Func<A, B>> mf, WriterT<W, M, A> ma)  
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> => 
        mf.As().Bind(x =>ma.As().Map(x));

    public static WriterT<W, M, B> action<W, M, A, B>(WriterT<W, M, A> ma, WriterT<W, M, B> mb) 
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> => 
        ma.As().Bind(_ => mb);

    public static WriterT<W, M, A> lift<W, M, A>(K<M, A> ma)  
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> => 
        WriterT<W, M, A>.Lift(ma);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> liftIO<W, M, A>(IO<A> effect)
        where W : Monoid<W>
        where M : Monad<M>, SemiAlternative<M> =>
        WriterT<W, M, A>.LiftIO(effect);

    /// <summary>
    /// Tell is an action that produces the writer output
    /// </summary>
    /// <param name="item">Item to tell</param>
    /// <typeparam name="W">Writer type</typeparam>
    /// <returns>Structure with the told item</returns>
    public static WriterT<W, M, Unit> tell<M, W>(W item)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        new (w => M.Pure((default(Unit), w + item)));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static WriterT<W, M, A> write<M, W, A>((A, W) item)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M> 
        where W : Monoid<W> =>
        new (w => M.Pure((item.Item1, w + item.Item2)));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static WriterT<W, M, A> write<M, W, A>(A value, W item)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M> 
        where W : Monoid<W> =>
        new (w => M.Pure((value, w + item)));

    /// <summary>
    /// `pass` is an action that executes the `action`, which returns a value and a
    /// function; it then returns a the value with the output having been applied to
    /// the function.
    /// </summary>
    public static WriterT<W, M, A> pass<M, W, A>(WriterT<W, M, (A Value, Func<W, W> Function)> action)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M> 
        where W : Monoid<W> =>
        WriterM.pass(action).As();

    /// <summary>
    /// `listen` is executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static WriterT<W, M, (A Value, W Output)> listen<M, W, A>(WriterT<W, M, A> ma)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        WriterM.listen<WriterT<W, M>, W, A>(ma).As();

    /// <summary>
    /// `listens` is executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static WriterT<W, M, (A Value, B Output)> listens<M, W, A, B>(Func<W, B> f, WriterT<W, M, A> ma)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M>
        where W : Monoid<W> =>
        WriterM.listens(f, ma).As();

    /// <summary>
    /// `censor` is executes the action `ma` and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public static WriterT<W, M, A> censor<M, W, A>(Func<W, W> f, WriterT<W, M, A> ma)
        where M : WriterM<M, W>, Monad<M>, SemiAlternative<M> 
        where W : Monoid<W> =>
        WriterM.censor(f, ma).As();
}
