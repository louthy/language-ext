using System;

namespace LanguageExt.Traits;

/// <summary>
/// The class of contravariant functors.
/// Whereas one can think of a `Functor` as containing or producing values, a contravariant functor is a functor that
/// can be thought of as consuming values.
/// 
/// Contravariant functors are referred to colloquially as Cofunctor, even though the dual of a `Functor` is just
/// a `Functor`. 
/// </summary>
/// <typeparam name="F">Self-referring type</typeparam>
public interface Cofunctor<F>
{
    public static abstract K<F, A> Comap<A, B>(Func<A, B> f, K<F, B> fb);
}
