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
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract K<F, A> Pure<A>(A value);
    public static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);

    public static virtual K<F, B> ApplyLazy<A, B>(K<F, Func<A, B>> mf, Func<K<F, A>> ma) =>
        mf * ma();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //

    public static virtual K<F, B> Action<A, B>(K<F, A> ma, K<F, B> mb) =>
        ((A _, B y) => y) * ma * mb;
    
    public static virtual K<F, C> Apply<A, B, C>(K<F, Func<A, B, C>> mf, K<F, A> ma, K<F, B> mb) =>
        curry * mf * ma * mb;

    public static virtual K<F, Func<B, C>> Apply<A, B, C>(K<F, Func<A, B, C>> mf, K<F, A> ma) =>
        curry * mf * ma;

    public static virtual K<F, C> Apply<A, B, C>(K<F, Func<A, Func<B, C>>> mf, K<F, A> ma, K<F, B> mb) =>
        mf * ma * mb;

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
