using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface Alternative<F> : Choice<F>, Applicative<F>
    where F : Alternative<F>
{
    /// <summary>
    /// Identity
    /// </summary>
    [Pure]
    public static abstract K<F, A> Empty<A>(); 
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static virtual K<F, A> Choice<A>(in Seq<K<F, A>> ms)
    {
        if(ms.IsEmpty) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms.Tail)
        {
            r |= m;
        }
        return r;
    }

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static virtual K<F, A> Choice<A>(in ReadOnlySpan<K<F, A>> ms)
    {
        if(ms.Length == 0) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms)
        {
            r |= m;
        }
        return r;
    }
        
    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Some<A>(K<F, A> fa)
    {
        return some();
        
        K<F, Seq<A>> many() =>
            F.Choose(some(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some() =>
            Cached<A>.cons * fa * memoK(many);
    }
    
    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Many<A>(K<F, A> fa)
    {
        return many();
        
        K<F, Seq<A>> many() =>
            some() | F.Pure(Seq<A>());

        K<F, Seq<A>> some() =>
            Cached<A>.cons * fa * memoK(many);
    }
    
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
    public static virtual K<F, Seq<A>> EndBy<A, SEP>(K<F, A> p, K<F, SEP> sep) =>
        F.Many(p.BackAction(sep));
    
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
    public static virtual K<F, Seq<A>> EndBy1<A, SEP>(K<F, A> p, K<F, SEP> sep) =>
        F.Some(p.BackAction(sep));

    /// <summary>
    /// Combine two alternatives
    /// </summary>
    /// <param name="fa">Left alternative</param>
    /// <param name="fb">Right alternative</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Alternative structure with an `Either` lifted into it</returns>
    [Pure]
    public static virtual K<F, Either<A, B>> Either<A, B>(K<F, A> fa, K<F, B> fb) =>
        (Left<A, B>) * fa | (Right<A, B>) * fb; 
    
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
    public static virtual K<F, Seq<A>> ManyUntil<A, END>(K<F, A> fa, K<F, END> fend)
    {
        var empty = Prelude.Pure(Seq<A>.Empty);
        return go();

        K<F, Seq<A>> go() =>
            empty * fend | Applicative.lift(Seq.cons, fa, memoK(go));
    }
        
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
    public static virtual K<F, (Seq<A> Items, END End)> ManyUntil2<A, END>(K<F, A> fa, K<F, END> fend)
    {
        var empty = (END e) => (Seq<A>.Empty, e);
        return go();

        K<F, (Seq<A> Items, END End)> go() =>
            empty * fend | Applicative.lift((x, p) => (x.Cons(p.Items), p.End), fa, memoK(go));
    }
    
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
    public static virtual K<F, Seq<A>> SomeUntil<A, END>(K<F, A> fa, K<F, END> fend) =>
        Applicative.lift(Seq.cons, fa, F.ManyUntil(fa, fend));
        
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
    public static virtual K<F, (Seq<A> Items, END End)> SomeUntil2<A, END>(K<F, A> fa, K<F, END> fend) =>
        Applicative.lift((x, p) => (x.Cons(p.Items), p.End), fa, F.ManyUntil2(fa, fend));
    
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
    /// <param name="p"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    public static virtual K<F, A> Option<A>(A value, K<F, A> fa) => 
        fa | F.Pure(value);
    
    /// <summary>
    /// `sepBy(fa, sep) processes _zero_ or more occurrences of `fa`, separated by `sep`. 
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="fsep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static virtual K<F, Seq<A>> SepBy<A, SEP>(K<F, A> fa, K<F, SEP> sep) =>
        F.SepBy1(fa, sep) | F.Pure(Seq<A>.Empty);
    
    /// <summary>
    /// `sepBy(fa, sep) processes _one_ or more occurrences of `fa`, separated by `sep`. 
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="fsep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static virtual K<F, Seq<A>> SepBy1<A, SEP>(K<F, A> fa, K<F, SEP> sep) =>
        Applicative.lift(Seq.cons, fa, F.Many(sep >>> fa));

    /// <summary>
    /// `sepEndBy(fa, sep) processes _zero_ or more occurrences of `fa`, separated
    /// and optionally ended by `sep`. Returns a list of values returned by `fa`.
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="fsep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static virtual K<F, Seq<A>> SepByEnd<A, SEP>(K<F, A> fa, K<F, SEP> sep) =>
        F.SepByEnd1(fa, sep) | F.Pure(Seq<A>.Empty);

    /// <summary>
    /// `sepEndBy1(fa, sep) processes _one_ or more occurrences of `fa`, separated
    /// and optionally ended by `sep`. Returns a list of values returned by `fa`.
    /// </summary>
    /// <param name="fa">Structure to yield return values</param>
    /// <param name="fsep">Separator structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="SEP">Separator type</typeparam>
    /// <returns>List of values returned by `fa`</returns>
    [Pure]
    public static virtual K<F, Seq<A>> SepByEnd1<A, SEP>(K<F, A> fa, K<F, SEP> sep) =>
        Applicative.lift(Seq.cons, fa, sep >>> F.SepByEnd(fa, sep) | F.Pure(Seq<A>.Empty));
    
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
    public static virtual K<F, Unit> SkipMany<A>(K<F, A> fa)
    {
        return go();
        K<F, Unit> go() =>
            fa >> memoK(go) | F.Pure(unit);
    }
    
    /// <summary>
    /// Process `fa` _one_ or more times and drop all yielded values.
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly until failure. At least one item must be yielded for overall success.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Unit</returns>
    [Pure]
    public static virtual K<F, Unit> SkipSome<A>(K<F, A> fa) =>
        fa >>> F.SkipMany(fa);

    /// <summary>
    /// `skip(n, fa)` processes `n` occurrences of `fa`, skipping its result.
    /// If `n` is not positive, the process equates to `Pure(unit)`.
    /// </summary>
    /// <param name="n">Number of occurrences of `fa` to skip</param>
    /// <param name="fa">Applicative functor</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static virtual K<F, Unit> Skip<A>(int n, K<F, A> fa) =>
        n switch
        {
            <= 0 => F.Pure(unit),
            _    => Applicative.lift((_, _) => unit, fa, F.Replicate(n - 1, fa))
        };

    /// <summary>
    /// `skipManyUntil(fa, fend)` applies the process `fa` _zero_ or more times
    /// skipping results until process `fend` succeeds. The resulting value from
    /// `fend` is then returned.
    /// </summary>
    /// <param name="n">Number of occurrences of `fa` to skip</param>
    /// <param name="fa">Applicative functor</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static virtual K<F, END> SkipManyUntil<A, END>(K<F, A> fa, K<F, END> fend)
    {
        return go();
        K<F, END> go() =>
            fend | fa >> memoK(go); 
    }

    /// <summary>
    /// `skipManyUntil(fa, fend)` applies the process `fa` _one_ or more times
    /// skipping results until process `fend` succeeds. The resulting value from
    /// `fend` is then returned.
    /// </summary>
    /// <param name="n">Number of occurrences of `fa` to skip</param>
    /// <param name="fa">Applicative functor</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    [Pure]
    public static virtual K<F, END> SkipSomeUntil<A, END>(K<F, A> fa, K<F, END> fend) =>
        fa >>> F.SkipManyUntil(fa, fend);
        
    static class Cached<A>
    {
        public static readonly Func<A, Func<Seq<A>, Seq<A>>> cons =
            static x => xs => x.Cons(xs);
    }
}
