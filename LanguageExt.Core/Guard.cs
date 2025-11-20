using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Used by various error-producing monads to have a contextual `where`
/// </summary>
/// <remarks>
/// See `Prelude.guard(...)`
/// </remarks>
public readonly struct Guard<E, A>
{
    public readonly bool Flag;
    readonly Func<E> onFalse;

    internal Guard(bool flag, Func<E> onFalse) =>
        (Flag, this.onFalse) = (flag, onFalse ?? throw new ArgumentNullException(nameof(onFalse)));

    internal Guard(bool flag, E onFalse)
    {
        if (isnull(onFalse)) throw new ArgumentNullException(nameof(onFalse));
        (Flag, this.onFalse) = (flag, () => onFalse);
    }

    public Guard<E, B> Cast<B>() =>
        new (Flag, OnFalse);
        
    public Func<E> OnFalse =>
        onFalse ?? throw new InvalidOperationException(
            "Guard isn't initialised. It was probably created via new Guard() or default(Guard), and so it has no OnFalse handler");

    public Guard<E, C> SelectMany<C>(Func<E, Guard<E, Unit>> bind, Func<Unit, Unit, C> project) =>
        Flag ? bind(default!).Cast<C>() : Cast<C>();

    public Guard<E, B> Select<B>(Func<B, B> _) =>
        Cast<B>();
}
