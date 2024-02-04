using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, Console.writeLine<RT>("x should be 100!"))
    ///     select x;
    /// 
    /// </example>
    public static Eff<RT, Unit> unless<RT>(bool flag, Eff<RT, Unit> alternative) where RT : HasIO<RT, Error> =>
        when(!flag, alternative);

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, Console.writeLine<RT>("x should be 100!"))
    ///     select x;
    /// 
    /// </example>
    public static Eff<Unit> unless(bool flag, Eff<Unit> alternative) =>
        when(!flag, alternative);        

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, None)
    ///     select x;
    /// 
    /// </example>
    public static Option<Unit> unless(bool flag, Option<Unit> alternative) =>
        when(!flag, alternative);    

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, Left<string, int>("100 is the only value accepted"))
    ///     select x;
    /// 
    /// </example>
    public static Either<L, Unit> unless<L>(bool flag, Either<L, Unit> alternative) =>
        when(!flag, alternative);      

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, FinFail(Error.New("100 is the only value accepted"))
    ///     select x;
    /// 
    /// </example>
    public static Fin<Unit> unless(bool flag, Fin<Unit> alternative) =>
        when(!flag, alternative);

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, async () => await LaunchMissiles())
    ///     select x;
    /// 
    /// </example>
    public static Task<Unit> unless(bool flag, Task<Unit> alternative) =>
        when(!flag, alternative); 

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, async () => await LaunchMissiles())
    ///     select x;
    /// 
    /// </example>
    public static ValueTask<Unit> unless(bool flag, ValueTask<Unit> alternative) =>
        when(!flag, alternative); 

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, Fail<string, Unit>("100 is the only value accepted"))
    ///     select x;
    /// 
    /// </example>
    public static Validation<L, Unit> unless<L>(bool flag, Validation<L, Unit> alternative) =>
        when(!flag, alternative);      

    /// <summary>
    /// Run the `alternative` when the `flag` is `false`, return `Pure Unit` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, Fail<MString, string, Unit>("100 is the only value accepted"))
    ///     select x;
    /// 
    /// </example>
    public static Validation<MonoidL, L, Unit> unless<MonoidL, L>(bool flag, Validation<MonoidL, L, Unit> alternative) 
        where MonoidL : Monoid<L>, Eq<L> =>
        when(!flag, alternative);       
}
