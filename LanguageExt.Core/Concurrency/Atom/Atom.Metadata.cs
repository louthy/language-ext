using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
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

        public event AtomChangedEvent<A> Change;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Atom(M metadata, A value, Func<A, bool> validator)
        {
            this.value = Box<A>.New(value);
            this.metadata = metadata;
            this.validator = validator;
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
            new Atom<M, A>(metadata, value, True);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
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
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Eff<A> SwapEff(Func<M, A, Eff<A>> f) =>
            EffMaybe<A>(() =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = f(metadata, Box<A>.GetValue(value)).Run();
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }
                    if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            }); 
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Eff<RT, A> SwapEff<RT>(Func<M, A, Eff<RT, A>> f) where RT : struct =>
            EffMaybe<RT, A>(env =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = f(metadata, Box<A>.GetValue(value)).Run(env);
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }
                    if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            });        

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public async ValueTask<Option<A>> SwapAsync(Func<M, A, ValueTask<A>> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            SpinWait sw = default;
            while (true)
            {
                var current = value;
                var newValueA = await f(metadata, Box<A>.GetValue(value)).ConfigureAwait(false);
                var newValue = Box<A>.New(newValueA);
                if (!validator(newValueA))
                {
                    return default;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
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
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<A> SwapAff(Func<M, A, Aff<A>> f) =>
            AffMaybe<A>(async () =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = await f(metadata, Box<A>.GetValue(value)).Run().ConfigureAwait(false);
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }

                    sw.SpinOnce();
                }
            });         
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<A> SwapAff(Func<M, A, ValueTask<A>> f) =>
            AffMaybe<A>(async () =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueA = await f(metadata, Box<A>.GetValue(value)).ConfigureAwait(false);

                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return FinSucc<A>(newValueA);
                    }
                    sw.SpinOnce();
                }
            });         
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<RT, A> SwapAff<RT>(Func<M, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async env =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (!env.CancellationToken.IsCancellationRequested)
                {
                    var current = value;
                    var newFinValueA = await f(metadata, Box<A>.GetValue(value)).Run(env).ConfigureAwait(false);
                    if (newFinValueA.IsFail)
                    {
                        return newFinValueA;
                    }

                    var newValueA = newFinValueA.Value; 

                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newFinValueA;
                    }

                    sw.SpinOnce();
                }

                return Errors.Cancelled;
            });

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public Option<A> Swap<X>(X x, Func<M, X, A, A> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            SpinWait sw = default;
            while (true)
            {
                var current = value;
                var newValueA = f(metadata, x, Box<A>.GetValue(value));
                var newValue = Box<A>.New(newValueA);
                if (!validator(newValueA))
                {
                    return default;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
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
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Eff<A> SwapEff<X>(X x, Func<M, X, A, Eff<A>> f) =>
            EffMaybe<A>(() =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = f(metadata, x, Box<A>.GetValue(value)).Run();
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }
                    if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            });         
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Eff<RT, A> SwapEff<RT, X>(X x, Func<M, X, A, Eff<RT, A>> f) where RT : struct =>
            EffMaybe<RT, A>(env =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = f(metadata, x, Box<A>.GetValue(value)).Run(env);
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }
                    if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            });        

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public async ValueTask<Option<A>> SwapAsync<X>(X x, Func<M, X, A, ValueTask<A>> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            SpinWait sw = default;
            while (true)
            {
                var current = value;
                var newValueA = await f(metadata, x, Box<A>.GetValue(value)).ConfigureAwait(false);
                var newValue = Box<A>.New(newValueA);
                if (!validator(newValueA))
                {
                    return default;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
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
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<A> SwapAff<X>(X x, Func<M, X, A, Aff<A>> f) =>
            AffMaybe<A>(async () =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = await f(metadata, x, Box<A>.GetValue(value)).Run().ConfigureAwait(false);
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            });         
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<A> SwapAff<X>(X x, Func<M, X, A, ValueTask<A>> f) =>
            AffMaybe<A>(async () =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueA = await f(metadata, x, Box<A>.GetValue(value)).ConfigureAwait(false);

                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return FinSucc<A>(newValueA);
                    }

                    sw.SpinOnce();
                }
            });         
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<RT, A> SwapAff<RT, X>(X x, Func<M, X, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async env =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (!env.CancellationToken.IsCancellationRequested)
                {
                    var current = value;
                    var newFinValueA = await f(metadata, x, Box<A>.GetValue(value)).Run(env).ConfigureAwait(false);
                    if (newFinValueA.IsFail)
                    {
                        return newFinValueA;
                    }

                    var newValueA = newFinValueA.Value; 

                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newFinValueA;
                    }

                    sw.SpinOnce();
                }

                return Errors.Cancelled;
            });        

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public Option<A> Swap<X, Y>(X x, Y y, Func<M, X, Y, A, A> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            SpinWait sw = default;
            while (true)
            {
                var current = value;
                var newValueA = f(metadata, x, y, Box<A>.GetValue(value));
                var newValue = Box<A>.New(newValueA);
                if (!validator(newValueA))
                {
                    return default;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
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
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Eff<A> SwapEff<X, Y>(X x, Y y, Func<M, X, Y, A, Eff<A>> f) =>
            EffMaybe<A>(() =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = f(metadata, x, y, Box<A>.GetValue(value)).Run();
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }
                    if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            });   
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Eff<RT, A> SwapEff<RT, X, Y>(X x, Y y, Func<M, X, Y, A, Eff<RT, A>> f) where RT : struct =>
            EffMaybe<RT, A>(env =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = f(metadata, x, y, Box<A>.GetValue(value)).Run(env);
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }
                    if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }
                    sw.SpinOnce();
                }
            });        

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public async ValueTask<Option<A>> SwapAsync<X, Y>(X x, Y y, Func<M, X, Y, A, ValueTask<A>> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            SpinWait sw = default;
            while (true)
            {
                var current = value;
                var newValueA = await f(metadata, x, y, Box<A>.GetValue(value)).ConfigureAwait(false);
                var newValue = Box<A>.New(newValueA);
                if (!validator(newValueA))
                {
                    return default;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
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
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<A> SwapAff<X, Y>(X x, Y y, Func<M, X, Y, A, Aff<A>> f) =>
            AffMaybe<A>(async () =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueFinA = await f(metadata, x, y, Box<A>.GetValue(value)).Run().ConfigureAwait(false);
                    if (newValueFinA.IsFail)
                    {
                        return newValueFinA;
                    }

                    var newValueA = newValueFinA.Value;
                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newValueFinA;
                    }

                    sw.SpinOnce();
                }
            });         
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<A> SwapAff<X, Y>(X x, Y y, Func<M, X, Y, A, ValueTask<A>> f) =>
            AffMaybe<A>(async () =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (true)
                {
                    var current = value;
                    var newValueA = await f(metadata, x, y, Box<A>.GetValue(value)).ConfigureAwait(false);

                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return FinSucc<A>(newValueA);
                    }

                    sw.SpinOnce();
                }
            }); 
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public Aff<RT, A> SwapAff<RT, X, Y>(X x, Y y, Func<M, X, Y, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async env =>
            {
                f = f ?? throw new ArgumentNullException(nameof(f));

                SpinWait sw = default;
                while (!env.CancellationToken.IsCancellationRequested)
                {
                    var current = value;
                    var newFinValueA = await f(metadata, x, y, Box<A>.GetValue(value)).Run(env).ConfigureAwait(false);
                    if (newFinValueA.IsFail)
                    {
                        return newFinValueA;
                    }

                    var newValueA = newFinValueA.Value; 

                    var newValue = Box<A>.New(newValueA);
                    if (!validator(newValueA))
                    {
                        return FinFail<A>(Error.New("Validation failed for swap"));
                    }

                    if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                    {
                        Change?.Invoke(newValueA);
                        return newFinValueA;
                    }

                    sw.SpinOnce();
                }

                return Errors.Cancelled;
            });        

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
            Value.ToString();

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
}
