using System;
using LanguageExt.Traits;

namespace LanguageExt.Free;

/// <summary>
/// Free monad makes any functor into a monad 
/// </summary>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record Free<F, A> : K<Free<F>, A>
    where F : Functor<F>;

/// <summary>
/// Terminal case for the free monad
/// </summary>
/// <param name="Value">Terminal value</param>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public sealed record Pure<F, A>(A Value) : Free<F, A>
    where F : Functor<F>;

/// <summary>
/// Monadic bind case for the free monad
/// </summary>
/// <param name="Value">Functor that yields a `Free` monad</param>
/// <typeparam name="F">Functor type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public sealed record Bind<F, A>(K<F, Free<F, A>> Value) : Free<F, A>
    where F : Functor<F>;
