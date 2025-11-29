using System;
using System.Collections.Generic;
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
    static Func<A, Func<B, B>> action<A, B>() =>
        Act<A, B>.fun;        
    
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
    public static virtual K<F, B> Action<A, B>(K<F, A> ma, K<F, B> mb) =>
        action<A, B>() * ma * mb;

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    public static virtual K<F, B> Action<A, B>(K<F, A> ma, Memo<F, B> mb) =>
        action<A, B>() * ma * mb;

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    public static virtual K<F, B> Action<A, B>(Memo<F, A> ma, Memo<F, B> mb) =>
        action<A, B>() * ma * mb;

    /// <summary>
    /// Applicative action.  Computes the first applicative action and then computes the second.
    /// </summary>
    /// <param name="ma">First applicative structure</param>
    /// <param name="mb">Second applicative structure</param>
    /// <typeparam name="A">First applicative structure bound value type</typeparam>
    /// <typeparam name="B">Second applicative structure bound value type</typeparam>
    /// <returns>The result of the second applicative action (if there wasn't a failure beforehand)</returns>
    public static virtual K<F, B> Action<A, B>(Memo<F, A> ma, K<F, B> mb) =>
        action<A, B>() * ma * mb;

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
    public static virtual K<F, A> Actions<A>(params K<F, A>[] fas) =>
        F.Actions(fas.AsEnumerable());

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
    public static virtual K<F, A> Actions<A>(Seq<K<F, A>> fas) =>
        F.Actions(fas.AsEnumerable());

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
    public static virtual K<F, A> Actions<A>(IEnumerable<K<F, A>> fas)
    {
        K<F, A>? ra = null;
        foreach (var fa in fas)
        {
            ra = ra is null
                     ? fa
                     : F.Action(ra, fa);
        }

        if (ra is null) throw Exceptions.SequenceEmpty;
        return ra;
    }

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
    ///                 rt =〉 fas.Select(fa =〉 fa.RunIO(rt)).Actions()));
    ///  
    /// </remarks>
    /// <param name="fas">Actions to chain</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    /// <exception cref="ExpectedException">Sequence is empty</exception>
    public static virtual K<F, A> Actions<A>(IAsyncEnumerable<K<F, A>> fas) =>
        F.Actions(fas.ToBlockingEnumerable());
}
