using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Console.writeLine<RT>("x is 100, finally!"))
    ///     select x;
    /// 
    /// </example>
    public static Eff<RT, Unit> when<RT>(bool flag, Eff<RT, Unit> alternative) where RT : HasIO<RT, Error> =>
        flag
            ? alternative
            : SuccessEff(unit);
        
    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Console.writeLine<RT>("x is 100, finally!"))
    ///     select x;
    /// 
    /// </example>
    public static Eff<Unit> when(bool flag, Eff<Unit> alternative) =>
        flag
            ? alternative
            : SuccessEff(unit);
        
    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, None)
    ///     select x;
    /// 
    /// </example>
    public static Option<Unit> when(bool flag, Option<Unit> alternative) =>
        flag
            ? alternative
            : Some(unit);
        
    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Left<string, int>("Won't accept 100"))
    ///     select x;
    /// 
    /// </example>
    public static Either<L, Unit> when<L>(bool flag, Either<L, Unit> alternative) =>
        flag
            ? alternative
            : Right<L, Unit>(unit);
        
    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, FinFail(Error.New("Won't accept 100")))
    ///     select x;
    /// 
    /// </example>
    public static Fin<Unit> when(bool flag, Fin<Unit> alternative) =>
        flag
            ? alternative
            : FinSucc(unit);

    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, async () => await LaunchMissiles())
    ///     select x;
    /// 
    /// </example>
    public static Task<Unit> when(bool flag, Task<Unit> alternative) =>
        flag
            ? alternative
            : unit.AsTask();


    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, async () => await LaunchMissiles())
    ///     select x;
    /// 
    /// </example>
    public static ValueTask<Unit> when(bool flag, ValueTask<Unit> alternative) =>
        flag
            ? alternative
            : unit.AsValueTask();
        
    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Fail<string, Unit>("100 isn't accepted"))
    ///     select x;
    /// 
    /// </example>
    public static Validation<L, Unit> when<L>(bool flag, Validation<L, Unit> alternative) =>
        flag
            ? alternative
            : Success<L, Unit>(unit);

    /// <summary>
    /// Run the `alternative` when the `flag` is `true`, return `Pure Unit` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Fail<MString, string, Unit>("100 isn't accepted"))
    ///     select x;
    /// 
    /// </example>
    public static Validation<MonoidL, L, Unit> when<MonoidL, L>(bool flag, Validation<MonoidL, L, Unit> alternative) 
        where MonoidL : Monoid<L>, Eq<L> =>
        flag
            ? alternative
            : Success<MonoidL, L, Unit>(unit);
}
