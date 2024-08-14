using System;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Atoms provide a way to manage shared, synchronous, independent state without 
/// locks. 
/// </summary>
/// <remarks>
/// The intended use of atom is to hold an immutable data structure. You change 
/// the value by applying a function to the old value. This is done in an atomic 
/// manner by `Swap`.  
/// 
/// Internally, `Swap` reads the current value, applies the function to it, and 
/// attempts to `CompareExchange` it in. Since another thread may have changed the 
/// value in the intervening time, it may have to retry, and does so in a spin loop. 
/// 
/// The net effect is that the value will always be the result of the application 
/// of the supplied function to a current value, atomically. However, because the 
/// function might be called multiple times, it must be free of side effects.
/// 
/// Atoms are an efficient way to represent some state that will never need to be 
/// coordinated with any other, and for which you wish to make synchronous changes.
/// </remarks>
/// <remarks>
/// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
/// </remarks>
public sealed class Atom<M, A>
{
    volatile object value;
    Func<A, bool> validator;
    readonly M metadata;

    public event AtomChangedEvent<A>? Change;

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    Atom(M metadata, A value, Func<A, bool>? validator)
    {
        this.value     = Box<A>.New(value);
        this.metadata  = metadata;
        this.validator = validator ?? True;
    }

    /// <summary>
    /// Internal constructor function that runs the validator on the value
    /// before returning the Atom so that the Atom can never be in an invalid
    /// state.  The validator is then used for all state transitions going 
    /// forward.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Option<Atom<M, A>> New(M metadata, A value, Func<A, bool> validator)
    {
        var atom = new Atom<M, A>(metadata, value, validator ?? throw new ArgumentNullException(nameof(validator)));
        return validator(value)
                   ? Some(atom)
                   : None;
    }

    /// <summary>
    /// Internal constructor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Atom<M, A> New(M metadata, A value) =>
        new (metadata, value, True);

    /// <summary>
    /// Atomically updates the value by passing the old value to `f` and updating
    /// the atom with the result.  Note: `f` may be called multiple times, so it
    /// should be free of side effects.
    /// </summary>
    /// <param name="f">Function to update the atom</param>
    /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
    /// and its validation passed. None otherwise</returns>
    public Option<A> Swap(Func<M, A, A> f)
    {
        f = f ?? throw new ArgumentNullException(nameof(f));

        SpinWait sw = default;
        while (true)
        {
            var current = value;
            var newValueA = f(metadata, Box<A>.GetValue(value));
            var newValue = Box<A>.New(newValueA);
            if (!validator(newValueA))
            {
                return default;
            }
            if(Interlocked.CompareExchange(ref value, newValue, current) == current)
            {
                Change?.Invoke(newValueA);
                return Optional(newValueA);
            }
            sw.SpinOnce();
        }
    }

    /// <summary>
    /// Atomically updates the value by passing the old value to `f` and updating
    /// the atom with the result.  Note: `f` may be called multiple times, so it
    /// should be free of side effects.
    /// </summary>
    /// <param name="x">Additional value to pass to `f`</param>
    /// <param name="f">Function to update the atom</param>
    /// <returns>IO in a success state, with the result of the invocation of `f`, if the swap succeeded and its
    /// validation passed. Failure state otherwise</returns>
    public IO<A> SwapIO(Func<M, A, A> f) =>
        IO.lift(_ => Swap(f) switch
                     {
                         { IsSome: true } value => (A)value,
                         _                      => throw Exceptions.ValidationFailed
                     });

    /// <summary>
    /// Value accessor (read and write)
    /// </summary>
    /// <remarks>
    /// 
    /// * Gets will return a freshly constructed `IO` monad that can be repeatedly
    /// evaluated to get the latest state of the `Atom`.
    /// 
    /// * Sets pass an `IO` monad that will be mapped to an operation that will set
    /// the value of the `Atom` each time it's evaluated.
    /// 
    /// </remarks>
    public IO<A> ValueIO
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IO.lift(_ => Value);
        set => value.Bind(v => SwapIO((_, _) => v));
    }

    /// <summary>
    /// Current state
    /// </summary>
    public A Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Box<A>.GetValue(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() =>
        Value?.ToString() ?? "[null]";

    /// <summary>
    /// Implicit conversion to `A`
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator A(Atom<M, A> atom) =>
        atom.Value;

    /// <summary>
    /// Helper for validator
    /// </summary>
    /// <param name="_"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool True(A _) => true;
}
