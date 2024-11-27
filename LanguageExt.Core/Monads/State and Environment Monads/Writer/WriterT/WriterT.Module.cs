using System;
using LanguageExt.Traits;

namespace LanguageExt;


public class WriterT<W>
    where W : Monoid<W>
{
    public static WriterT<W, M, A> lift<M, A>(K<M, A> ma)
        where M : Monad<M>, Choice<M> => 
        WriterT<W, M, A>.Lift(ma);
}

public partial class WriterT<W, M>
{
    public static WriterT<W, M, A> pure<A>(A value) => 
        WriterT<W, M, A>.Pure(value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> liftIO<A>(IO<A> effect) =>
        WriterT<W, M, A>.LiftIO(effect);
}

public class WriterT
{
    public static WriterT<W, M, A> pure<W, M, A>(A value)  
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> => 
        WriterT<W, M, A>.Pure(value);

    public static WriterT<W, M, A> lift<W, M, A>(K<M, A> ma)  
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> => 
        WriterT<W, M, A>.Lift(ma);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="effect">Monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> liftIO<W, M, A>(IO<A> effect)
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        WriterT<W, M, A>.LiftIO(effect);

    /// <summary>
    /// Tell is an action that produces the writer output
    /// </summary>
    /// <param name="item">Item to tell</param>
    /// <typeparam name="W">Writer type</typeparam>
    /// <returns>Structure with the told item</returns>
    public static WriterT<W, M, Unit> tell<W, M>(W item)
        where M : Monad<M>, Choice<M>
        where W : Monoid<W> =>
        new (w => M.Pure((default(Unit), w + item)));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static WriterT<W, M, A> write<W, M, A>((A, W) item)
        where M : Monad<M>, Choice<M> 
        where W : Monoid<W> =>
        new (w => M.Pure((item.Item1, w + item.Item2)));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static WriterT<W, M, A> write<W, M, A>(A value, W item)
        where M : Monad<M>, Choice<M> 
        where W : Monoid<W> =>
        new (w => M.Pure((value, w + item)));

    /// <summary>
    /// `pass` is an action that executes the `action`, which returns a value and a
    /// function; it then returns the value with the output having been applied to
    /// the function.
    /// </summary>
    public static WriterT<W, M, A> pass<W, M, A>(WriterT<W, M, (A Value, Func<W, W> Function)> action)
        where M : Monad<M>, Choice<M> 
        where W : Monoid<W> =>
        Writable.pass(action).As();

    /// <summary>
    /// `listen` executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static WriterT<W, M, (A Value, W Output)> listen<W, M, A>(WriterT<W, M, A> ma)
        where M : Monad<M>, Choice<M>
        where W : Monoid<W> =>
        Writable.listen<W, WriterT<W, M>, A>(ma).As();

    /// <summary>
    /// `listens` executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static WriterT<W, M, (A Value, B Output)> listens<W, M, A, B>(Func<W, B> f, WriterT<W, M, A> ma)
        where M : Monad<M>, Choice<M>
        where W : Monoid<W> =>
        Writable.listens(f, ma).As();

    /// <summary>
    /// `censor` executes the action `ma` and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public static WriterT<W, M, A> censor<W, M, A>(Func<W, W> f, WriterT<W, M, A> ma)
        where M : Monad<M>, Choice<M> 
        where W : Monoid<W> =>
        Writable.censor(f, ma).As();
}
