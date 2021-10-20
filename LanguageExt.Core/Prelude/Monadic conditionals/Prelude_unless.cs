using System;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
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
        public static Aff<RT, Unit> unless<RT>(bool flag, Aff<RT, Unit> alternative) where RT : struct, HasCancel<RT> =>
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
        public static Eff<RT, Unit> unless<RT>(bool flag, Eff<RT, Unit> alternative) where RT : struct, HasCancel<RT> =>
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
        public static Aff<Unit> unless(bool flag, Aff<Unit> alternative) =>
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
        ///     from _ in unless(x == 100, Try(() => WriteLineAsync<RT>("x should be 100!")))
        ///     select x;
        /// 
        /// </example>
        public static Try<Unit> unless(bool flag, Try<Unit> alternative) =>
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
        ///     from _ in unless(x == 100, TryAsync(() => WriteLineAsync<RT>("x should be 100!")))
        ///     select x;
        /// 
        /// </example>
        public static TryAsync<Unit> unless(bool flag, TryAsync<Unit> alternative) =>
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
        ///     from _ in unless(x == 100, TryOption(() => WriteLineAsync<RT>("x should be 100!")))
        ///     select x;
        /// 
        /// </example>
        public static TryOption<Unit> unless(bool flag, TryOption<Unit> alternative) =>
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
        ///     from _ in unless(x == 100, TryOptionAsync(() => WriteLineAsync<RT>("x should be 100!")))
        ///     select x;
        /// 
        /// </example>
        public static TryOptionAsync<Unit> unless(bool flag, TryOptionAsync<Unit> alternative) =>
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
        ///     from _ in unless(x == 100, None)
        ///     select x;
        /// 
        /// </example>
        public static OptionUnsafe<Unit> unless(bool flag, OptionUnsafe<Unit> alternative) =>
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
        public static OptionAsync<Unit> unless(bool flag, OptionAsync<Unit> alternative) =>
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
        ///     from _ in unless(x == 100, LeftUnsafe<string, int>("100 is the only value accepted"))
        ///     select x;
        /// 
        /// </example>
        public static EitherUnsafe<L, Unit> unless<L>(bool flag, EitherUnsafe<L, Unit> alternative) =>
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
        ///     from _ in unless(x == 100, LeftAsync<string, int>("100 is the only value accepted"))
        ///     select x;
        /// 
        /// </example>
        public static EitherAsync<L, Unit> unless<L>(bool flag, EitherAsync<L, Unit> alternative) =>
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
            where MonoidL : struct, Monoid<L>, Eq<L> =>
                when(!flag, alternative);       
    }
}
