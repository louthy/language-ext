using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static class Alternative
{
    /// <summary>
    /// Empty / none state for the `F` structure 
    /// </summary>
    /// <typeparam name="F">Alternative trait implementation</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Empty</returns>
    public static K<F, A> empty<F, A>()
        where F : Alternative<F> =>
        F.Empty<A>();

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> choice<F, A>(Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        F.Choice(ms);

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> choice<F, A>(params ReadOnlySpan<K<F, A>> ms)
        where F : Alternative<F> =>
        F.Choice(ms);

    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <remarks>
    /// NOTE: It is important that the `F` applicative-type overrides `Apply` (the one with `Func` laziness) in its
    /// trait-implementations otherwise this will likely result in a stack-overflow. 
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static K<F, Seq<A>> some<F, A>(K<F, A> fa)
        where F : Alternative<F> =>
        F.Some(fa);
    
    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    /// Will always succeed.
    /// </remarks>
    /// <remarks>
    /// NOTE: It is important that the `F` applicative-type overrides `ApplyLazy` in its trait-implementations
    /// otherwise this will likely result in a stack-overflow. 
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static K<F, Seq<A>> many<F, A>(K<F, A> fa)
        where F : Alternative<F> =>
        F.Many(fa);
    
    /// <summary>
    /// `endBy(p, sep)` parses zero-or-more occurrences of `p`, separated and ended by
    /// `sep`. Returns a list of values returned by `p`.
    /// </summary>
    /// <param name="p">Value parser</param>
    /// <param name="sep">Separator parser</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, Seq<A>> endBy<F, A, SEP>(K<F, A> p, K<F, SEP> sep) 
        where F : Alternative<F> =>
        F.EndBy(p, sep);
    
    /// <summary>
    /// `endBy1(p, sep)` parses one-or-more occurrences of `p`, separated and ended by
    /// `sep`. Returns a list of values returned by `p`.
    /// </summary>
    /// <param name="p">Value parser</param>
    /// <param name="sep">Separator parser</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, Seq<A>> endBy1<F, A, SEP>(K<F, A> p, K<F, SEP> sep) 
        where F : Alternative<F> =>
        F.EndBy1(p, sep);
    
    /// <summary>
    /// Combine two alternatives
    /// </summary>
    /// <param name="ma">Left alternative</param>
    /// <param name="mb">Right alternative</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Alternative structure with an `Either` lifted into it</returns>
    [Pure]
    public static K<F, Either<A, B>> either<F, A, B>(K<F, A> ma, K<F, B> mb) 
        where F : Alternative<F> =>
        F.Either(ma, mb);
    
    /// <summary>
    /// `manyUntil(fa, end)` applies `fa` _zero_ or more times until `fend` succeeds.
    /// Returns the list of values returned by`fa`. `fend` result is consumed and
    /// lost. Use `manyUntil2` if you wish to keep it.
    /// </summary>
    /// <param name="fa">Structure to consume</param>
    /// <param name="fend">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, Seq<A>> manyUntil<F, A, END>(K<F, A> fa, K<F, END> fend)
        where F : Alternative<F> =>
        F.ManyUntil(fa, fend);
        
    /// <summary>
    /// `manyUntil2(fa, end)` applies `fa` _zero_ or more times until `fend` succeeds.
    /// Returns the list of values returned by`fa` plus the `fend` result.
    ///
    /// Use `manyUntil` if you don't wish to keep the `end` result.
    /// </summary>
    /// <param name="fa">Structure to consume</param>
    /// <param name="fend">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, (Seq<A> Items, END End)> manyUntil2<F, A, END>(K<F, A> fa, K<F, END> fend)
        where F : Alternative<F> =>
        F.ManyUntil2(fa, fend);
    
    /// <summary>
    /// `someUntil(fa, end)` applies `fa` _one_ or more times until `fend` succeeds.
    /// Returns the list of values returned by`fa`. `fend` result is consumed and
    /// lost. Use `someUntil2` if you wish to keep it.
    /// </summary>
    /// <param name="fa">Structure to consume</param>
    /// <param name="fend">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, Seq<A>> someUntil<F, A, END>(K<F, A> fa, K<F, END> fend) 
        where F : Alternative<F> =>
        F.SomeUntil(fa, fend);

    /// <summary>
    /// `someUntil2(fa, end)` applies `fa` _one_ or more times until `fend` succeeds.
    /// Returns the list of values returned by`fa` plus the `fend` result.
    ///
    /// Use `someUntil` if you don't wish to keep the `end` result.
    /// </summary>
    /// <param name="fa">Structure to consume</param>
    /// <param name="fend">Terminating structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, (Seq<A> Items, END End)> someUntil2<F, A, END>(K<F, A> fa, K<F, END> fend)
        where F : Alternative<F> =>
        F.SomeUntil2(fa, fend);
    
    /// <summary>
    /// `option(x, fa)` tries to apply `fa`. If `fa` fails without 'consuming' anything, it
    /// returns `value`, otherwise the value returned by `fa`.
    /// </summary>
    /// <remarks>
    /// The word 'consuming' is used here because this feature started life as a parser combinator, but it
    /// can be applied to any `Alternative` structure.  Critically, most combinators only have a single flavour
    /// of failure.  So, `option` just results in a default value being returned if `fa` fails.
    /// </remarks>
    /// <param name="value">Default value to use if `fa` fails without 'consuming' anything</param>
    /// <param name="fa"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, A> option<F, A>(A value, K<F, A> fa) 
        where F : Alternative<F> =>
        F.Option(value, fa);
    
    /// <summary>
    /// `sepBy(fa, sep) processes _zero_ or more occurrences of `fa`, separated by `sep`. 
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static K<F, Seq<A>> sepBy<F, A, SEP>(K<F, A> fa, K<F, SEP> sep) 
        where F : Alternative<F> =>
        F.SepBy(fa, sep);
    
    /// <summary>
    /// `sepBy(fa, sep) processes _one_ or more occurrences of `fa`, separated by `sep`. 
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static K<F, Seq<A>> sepBy1<F, A, SEP>(K<F, A> fa, K<F, SEP> sep) 
        where F : Alternative<F> =>
        F.SepBy1(fa, sep);

    /// <summary>
    /// `sepEndBy(fa, sep) processes _zero_ or more occurrences of `fa`, separated
    /// and optionally ended by `sep`. Returns a list of values returned by `fa`.
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static K<F, Seq<A>> sepByEnd<F, A, SEP>(K<F, A> fa, K<F, SEP> sep) 
        where F : Alternative<F> =>
        F.SepByEnd(fa, sep);

    /// <summary>
    /// `sepEndBy1(fa, sep) processes _one_ or more occurrences of `fa`, separated
    /// and optionally ended by `sep`. Returns a list of values returned by `fa`.
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="sep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static K<F, Seq<A>> sepByEnd1<F, A, SEP>(K<F, A> fa, K<F, SEP> sep) 
        where F : Alternative<F> =>
        F.SepByEnd1(fa, sep);
    
    /// <summary>
    /// Process `fa` _zero_ or more times and drop all yielded values.
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Unit</returns>
    [Pure]
    public static K<F, Unit> skipMany<F, A>(K<F, A> fa)
        where F : Alternative<F> =>
        F.SkipMany(fa);
    
    /// <summary>
    /// Process `fa` _one_ or more times and drop all yielded values.
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly until failure. At least one item must be yielded for overall success.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Unit</returns>
    [Pure]
    public static K<F, Unit> skipSome<F, A>(K<F, A> fa) 
        where F : Alternative<F> =>
        F.SkipSome(fa);

    /// <summary>
    /// `skip(n, fa)` processes `n` occurrences of `fa`, skipping its result.
    /// If `n` is not positive, the process equates to `Pure(unit)`.
    /// </summary>
    /// <param name="n">Number of occurrences of `fa` to skip</param>
    /// <param name="fa">Applicative functor</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, Unit> skip<F, A>(int n, K<F, A> fa) 
        where F : Alternative<F> =>
        F.Skip(n, fa);

    /// <summary>
    /// `skipManyUntil(fa, fend)` applies the process `fa` _zero_ or more times
    /// skipping results until process `fend` succeeds. The resulting value from
    /// `fend` is then returned.
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, END> skipManyUntil<F, A, END>(K<F, A> fa, K<F, END> fend)
        where F : Alternative<F> =>
        F.SkipManyUntil(fa, fend);

    /// <summary>
    /// `skipManyUntil(fa, fend)` applies the process `fa` _one_ or more times
    /// skipping results until process `fend` succeeds. The resulting value from
    /// `fend` is then returned.
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="END">End value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, END> skipSomeUntil<F, A, END>(K<F, A> fa, K<F, END> fend) 
        where F : Alternative<F> =>
        F.SkipSomeUntil(fa, fend);
}
