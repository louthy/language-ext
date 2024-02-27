using System;
using LanguageExt.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Writer<W>
{
    public static Writer<W, A> Pure<A>(A value) => 
        Writer<W, A>.Pure(value);
}

public class Writer
{
    public static Writer<W, A> Pure<W, A>(A value)  
        where W : Monoid<W> => 
        Writer<W, A>.Pure(value);

    public static Writer<W, B> bind<W, A, B>(Writer<W, A> ma, Func<A, Writer<W, B>> f) 
        where W : Monoid<W> => 
        ma.As().Bind(f);

    public static Writer<W, B> map<W, A, B>(Func<A, B> f, Writer<W, A> ma)  
        where W : Monoid<W> => 
        ma.As().Map(f);

    public static Writer<W, B> apply<W, A, B>(Writer<W, Func<A, B>> mf, Writer<W, A> ma)  
        where W : Monoid<W> => 
        mf.As().Bind(x =>ma.As().Map(x));

    public static Writer<W, B> action<W, A, B>(Writer<W, A> ma, Writer<W, B> mb) 
        where W : Monoid<W> => 
        ma.As().Bind(_ => mb);

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
        WriterM.write<Writer<W>, W, A>(item).As();

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static Writer<W, A> write<W, A>(A value, W item)
        where W : Monoid<W> =>
        WriterM.write<Writer<W>, W, A>(value, item).As();

    /// <summary>
    /// `pass` is an action that executes the `action`, which returns a value and a
    /// function; it then returns a the value with the output having been applied to
    /// the function.
    /// </summary>
    public static Writer<W, A> pass<W, A>(Writer<W, (A Value, Func<W, W> Function)> action)
        where W : Monoid<W> =>
        WriterM.pass(action).As();

    /// <summary>
    /// `listen` is executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static Writer<W, (A Value, W Output)> listen<W, A>(Writer<W, A> ma)
        where W : Monoid<W> =>
        WriterM.listen<Writer<W>, W, A>(ma).As();

    /// <summary>
    /// `listens` is executes the action `ma` and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public static Writer<W, (A Value, B Output)> listens<W, A, B>(Func<W, B> f, Writer<W, A> ma)
        where W : Monoid<W> =>
        WriterM.listens(f, ma).As();

    /// <summary>
    /// `censor` is executes the action `ma` and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public static Writer<W, A> censor<W, A>(Func<W, W> f, Writer<W, A> ma)
        where W : Monoid<W> =>
        WriterM.censor(f, ma).As();
}
