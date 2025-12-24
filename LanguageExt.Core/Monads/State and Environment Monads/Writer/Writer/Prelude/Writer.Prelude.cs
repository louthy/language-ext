using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Prelude
{
    /// <summary>
    /// Tell is an action that produces the writer output
    /// </summary>
    /// <param name="item">Item to tell</param>
    /// <typeparam name="W">Writer type</typeparam>
    /// <returns>Structure with the told item</returns>
    public static Writer<W, Unit> tell<W>(W item)
        where W : Monoid<W> =>
        new (w => (default, w + item));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static Writer<W, A> write<W, A>((A, W) item) 
        where W : Monoid<W> =>
        Writable.write<W, Writer<W>, A>(item).As();

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static Writer<W, A> write<W, A>(A value, W item)
        where W : Monoid<W> =>
        Writable.write<W, Writer<W>, A>(value, item).As();

    /// <summary>
    /// `pass` is an action that executes the `action`, which returns a value and a
    /// function; it then returns the value with the output having been applied to
    /// the function.
    /// </summary>
    public static Writer<W, A> pass<W, A>(Writer<W, (A Value, Func<W, W> Function)> action)
        where W : Monoid<W> =>
        Writable.pass(action).As();

    /// <summary>
    /// `listen` executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static Writer<W, (A Value, W Output)> listen<W, A>(Writer<W, A> ma)
        where W : Monoid<W> =>
        Writable.listen<W, Writer<W>, A>(ma).As();

    /// <summary>
    /// `listens` executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static Writer<W, (A Value, B Output)> listens<W, A, B>(Func<W, B> f, Writer<W, A> ma)
        where W : Monoid<W> =>
        Writable.listens(f, ma).As();

    /// <summary>
    /// `censor` executes the action `ma` and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public static Writer<W, A> censor<W, A>(Func<W, W> f, Writer<W, A> ma)
        where W : Monoid<W> =>
        Writable.censor(f, ma).As();
}
