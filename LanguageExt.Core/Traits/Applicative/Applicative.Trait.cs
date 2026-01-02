using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Applicative functor 
/// </summary>
/// <typeparam name="F">Functor trait type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Applicative<F> : Functor<F>
    where F : Applicative<F>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    /// <summary>
    /// Lift a pure value into the applicative structure
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Constructed applicative structure</returns>
    [Pure]
    public static abstract K<F, A> Pure<A>(A value);
    
    /// <summary>
    /// Apply the function to the argument.
    /// </summary>
    /// <remarks>
    /// This is like `delegate.Invoke` for lifted functions and lifted arguments.
    /// </remarks>
    /// <param name="mf">Lifted function</param>
    /// <param name="ma">Lifted argument</param>
    /// <typeparam name="A">Argument type</typeparam>
    /// <typeparam name="B">Return type</typeparam>
    /// <returns>Applicative structure that represents the result of invoking the lifted function with
    /// the lifted argument</returns>
    [Pure]
    public static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);
    
    /// <summary>
    /// Apply the function to the argument.
    /// This is like `delegate.Invoke` for lifted functions and lifted arguments.
    /// </summary>
    /// <remarks>
    /// Uses memoisation for lazy and then cached evaluation of the argument.
    /// </remarks>
    /// <param name="mf">Lifted function</param>
    /// <param name="ma">Lifted argument</param>
    /// <typeparam name="A">Argument type</typeparam>
    /// <typeparam name="B">Return type</typeparam>
    /// <returns>Applicative structure that represents the result of invoking the lifted function with
    /// the lifted argument</returns>
    [Pure]
    public static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, Memo<F, A> ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //

    /// <summary>
    /// Apply the function to the argument.
    /// This is like `delegate.Invoke` for lifted functions and lifted arguments.
    /// </summary>
    /// <remarks>
    /// Uses memoisation for lazy and then cached evaluation of the argument.
    /// </remarks>
    /// <param name="mf">Lifted function</param>
    /// <param name="ma">Lifted argument</param>
    /// <typeparam name="A">Argument type</typeparam>
    /// <typeparam name="B">Return type</typeparam>
    /// <returns>Applicative structure that represents the result of invoking the lifted function with
    /// the lifted argument</returns>
    [Pure]
    public static virtual K<F, B> Apply<A, B>(Memo<F, Func<A, B>> mf, Memo<F, A> ma) =>
        mf.Value.Apply(ma);

    /// <summary>
    /// Apply the function to the argument.
    /// This is like `delegate.Invoke` for lifted functions and lifted arguments.
    /// </summary>
    /// <remarks>
    /// Uses memoisation for lazy and then cached evaluation of the argument.
    /// </remarks>
    /// <param name="mf">Lifted function</param>
    /// <param name="ma">Lifted argument</param>
    /// <typeparam name="A">Argument type</typeparam>
    /// <typeparam name="B">Return type</typeparam>
    /// <returns>Applicative structure that represents the result of invoking the lifted function with
    /// the lifted argument</returns>
    [Pure]
    public static virtual K<F, B> Apply<A, B>(Memo<F, Func<A, B>> mf, K<F, A> ma) =>
        mf.Value.Apply(ma);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, B> Action<A, B>(K<F, A> ma, K<F, B> mb) =>
        Applicative.lift<F, A, B, B>(_ => y => y, ma, mb);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, A> BackAction<A, B>(K<F, A> ma, K<F, B> mb) =>
        Applicative.lift<F, A, B, A>(x => _ => x, ma, mb);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, B> Action<A, B>(K<F, A> ma, Memo<F, B> mb) =>
        Applicative.lift<F, A, B, B>(_ => y => y, memoK(ma), mb);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, A> BackAction<A, B>(K<F, A> ma, Memo<F, B> mb) =>
        Applicative.lift<F, A, B, A>(x => _ => x, memoK(ma), mb);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, B> Action<A, B>(Memo<F, A> ma, Memo<F, B> mb) =>
        Applicative.lift<F, A, B, B>(_ => y => y, ma, mb);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, A> BackAction<A, B>(Memo<F, A> ma, Memo<F, B> mb) =>
        Applicative.lift<F, A, B, A>(x => _ => x, ma, mb);

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, B> Action<A, B>(Memo<F, A> ma, K<F, B> mb) =>
        Applicative.lift<F, A, B, B>(_ => y => y, ma, memoK(mb));

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    [Pure]
    public static virtual K<F, A> BackAction<A, B>(Memo<F, A> ma, K<F, B> mb) =>
        Applicative.lift<F, A, B, A>(x => _ => x, ma, memoK(mb));

    /// <summary>
    /// Chains a sequence of applicative actions
    /// </summary>
    /// <remarks>
    /// Because this is an abstract chaining of actions, it can't actually run anything, and so if your
    /// actions are expected to have side effects (IO effects, for example), then you won't see them until
    /// the resulting `K〈F, A〉` is 'run'.
    ///
    /// This matters for infinite streams, where the result of `Actions` isn't realised at all, and so to
    /// avoid nothing happening (no side effects), you should override this function and unpack the IO
    /// type within, then run that enumerable of IOs.
    ///
    /// A good example is with the `Eff` type.  It's a `ReaderT〈IO, A〉` internally:
    ///
    ///     static K〈Eff〈RT〉, A〉 Actions〈A〉(IEnumerable〈K〈Eff〈RT〉, A〉〉 fas) =〉
    ///         new Eff〈RT, A〉(
    ///             new ReaderT〈RT, IO, A〉(
    ///                 rt =〉fas.Select(fa =〉fa.RunIO(rt)).Actions()));
    ///  
    /// </remarks>
    /// <param name="fas">Actions to chain</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    /// <exception cref="ExpectedException">Sequence is empty</exception>
    [Pure]
    public static virtual K<F, A> Actions<A>(IterableNE<K<F, A>> fas) =>
        // TODO: Consider ways to make this not be blocking as a sensible default implementation
        fas.Tail.Fold((h, t) => h.Action(t), fas.Head);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Supplementary members
    //
    
    /// <summary>
    /// `between(open, close, p) parses `open`, followed by `p` and `close`.
    /// </summary>
    /// <param name="open">Open computation</param>
    /// <param name="close">Close computation</param>
    /// <param name="p">Between computation</param>
    /// <typeparam name="A">Return value type</typeparam>
    /// <typeparam name="OPEN">OPEN value type</typeparam>
    /// <typeparam name="CLOSE">CLOSE value type</typeparam>
    /// <returns>The value returned by `p`</returns>
    [Pure]
    public static virtual K<F, A> Between<A, OPEN, CLOSE>(
        K<F, OPEN> open, 
        K<F, CLOSE> close, 
        K<F, A> p) =>
        open.Action(p).BackAction(close);
    
    /// <summary>
    /// Construct a sequence of `count` repetitions of `fa`
    /// </summary>
    /// <param name="count">Number of repetitions</param>
    /// <param name="fa">Applicative computation to run</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Applicative structure of `count` items</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Replicate<A>(int count, K<F, A> fa) =>
        count switch
        {
            <= 0 => F.Pure<Seq<A>>([]),
            _    => Applicative.lift(Seq.cons, fa, F.Replicate(count - 1, fa))
        };
}
