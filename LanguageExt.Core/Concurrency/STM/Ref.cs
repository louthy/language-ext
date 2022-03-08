using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Refs ensure safe shared use of mutable storage locations via a software transactional 
    /// memory (STM) system. Refs are bound to a single storage location for their lifetime, and 
    /// only allow mutation of that location to occur within a transaction.
    /// </summary>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    public sealed class Ref<A> : IEquatable<A>
    {
        internal readonly long Id;
        public event AtomChangedEvent<A> Change; 

        /// <summary>
        /// Internal ctor
        /// </summary>
        internal Ref(long id) =>
            Id = id;

        /// <summary>
        /// Destructor
        /// </summary>
        ~Ref() => STM.Finalise(Id);

        /// <summary>
        /// Change handler
        /// </summary>
        internal void OnChange(A value) =>
            Change?.Invoke(value);

        /// <summary>
        /// Value accessor
        /// </summary>
        public A Value
        {
            get => (A)STM.Read(Id);
            set => STM.Write(Id, value);
        }

        /// <summary>
        /// Implicit conversion operator
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator A(Ref<A> value) =>
            value.Value;

        /// <summary>
        /// ToString for the bound value
        /// </summary>
        public override string ToString() =>
            Value?.ToString() ?? "[null]";

        /// <summary>
        /// Hash code of the bound value
        /// </summary>
        public override int GetHashCode() =>
            Value?.GetHashCode() ?? 0;

        /// <summary>
        /// Equality
        /// </summary>
        public override bool Equals(object obj) =>
            obj is A val && Equals(val);

        /// <summary>
        /// Equality
        /// </summary>
        public bool Equals(A other) =>
            default(EqDefault<A>).Equals(other, Value);

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public A Swap(Func<A, A> f)
        {
            var v = f(Value);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public async ValueTask<A> SwapAsync(Func<A, ValueTask<A>> f)
        {
            var v = await f(Value).ConfigureAwait(false);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Aff<A> SwapAff(Func<A, Aff<A>> f) =>
            AffMaybe(async () =>
            {
                var fv = await f(Value).Run().ConfigureAwait(false);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Eff<A> SwapEff(Func<A, Eff<A>> f) =>
            EffMaybe(() =>
            {
                var fv = f(Value).Run();
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Aff<RT, A> SwapAff<RT>(Func<A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async env =>
            {
                var fv = await f(Value).Run(env).ConfigureAwait(false);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });
        
        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Eff<RT, A> SwapEff<RT>(Func<A, Eff<RT, A>> f) where RT : struct =>
            EffMaybe<RT, A>(env =>
            {
                var fv = f(Value).Run(env);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });
        
        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public A Swap<X>(X x, Func<X, A, A> f)
        {
            var v = f(x, Value);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public async ValueTask<A> SwapAsync<X, Y>(X x, Y y, Func<X, Y, A, ValueTask<A>> f)
        {
            var v = await f(x, y, Value).ConfigureAwait(false);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Aff<A> SwapAff<X, Y>(X x, Y y, Func<X, Y, A, Aff<A>> f) =>
            AffMaybe(async () =>
            {
                var fv = await f(x, y, Value).Run().ConfigureAwait(false);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Eff<A> SwapEff<X, Y>(X x, Y y, Func<X, Y, A, Eff<A>> f) =>
            EffMaybe(() =>
            {
                var fv = f(x, y, Value).Run();
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Aff<RT, A> SwapAff<X, Y, RT>(X x, Y y, Func<X, Y, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async env =>
            {
                var fv = await f(x, y, Value).Run(env).ConfigureAwait(false);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });
        
        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Eff<RT, A> SwapEff<X, Y, RT>(X x, Y y, Func<X, Y, A, Eff<RT, A>> f) where RT : struct =>
            EffMaybe<RT, A>(env =>
            {
                var fv = f(x, y, Value).Run(env);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });        

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public A Swap<X, Y>(X x, Y y, Func<X, Y, A, A> f)
        {
            var v = f(x, y, Value);
            Value = v;
            return v;
        }
        
        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public async ValueTask<A> SwapAsync<X>(X x, Func<X, A, ValueTask<A>> f)
        {
            var v = await f(x, Value).ConfigureAwait(false);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Aff<A> SwapAff<X>(X x, Func<X, A, Aff<A>> f) =>
            AffMaybe(async () =>
            {
                var fv = await f(x, Value).Run().ConfigureAwait(false);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Eff<A> SwapEff<X>(X x, Func<X, A, Eff<A>> f) =>
            EffMaybe(() =>
            {
                var fv = f(x, Value).Run();
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Aff<RT, A> SwapAff<X, RT>(X x, Func<X, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(async env =>
            {
                var fv = await f(x, Value).Run(env).ConfigureAwait(false);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });
        
        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public Eff<RT, A> SwapEff<X, RT>(X x, Func<X, A, Eff<RT, A>> f) where RT : struct =>
            EffMaybe<RT, A>(env =>
            {
                var fv = f(x, Value).Run(env);
                if (fv.IsFail) return fv;
                Value = fv.Value;
                return fv;
            });   

        /// <summary>
        /// Must be called in a transaction. Sets the in-transaction-value of
        /// ref to:  
        /// 
        ///     `f(in-transaction-value-of-ref)`
        ///     
        /// and returns the in-transaction-value when complete.
        /// 
        /// At the commit point of the transaction, `f` is run *AGAIN* with the
        /// most recently committed value:
        /// 
        ///     `f(most-recently-committed-value-of-ref)`
        /// 
        /// Thus `f` should be commutative, or, failing that, you must accept
        /// last-one-in-wins behavior.
        /// 
        /// Commute allows for more concurrency than just setting the Ref's value
        /// </summary>
        public CommuteRef<A> Commute(Func<A, A> f)
        {
            STM.Commute(Id, f);
            return new CommuteRef<A>(this);
        }

        /// <summary>
        /// Must be called in a transaction. Sets the in-transaction-value of
        /// ref to:  
        /// 
        ///     `f(in-transaction-value-of-ref)`
        ///     
        /// and returns the in-transaction-value when complete.
        /// 
        /// At the commit point of the transaction, `f` is run *AGAIN* with the
        /// most recently committed value:
        /// 
        ///     `f(most-recently-committed-value-of-ref)`
        /// 
        /// Thus `f` should be commutative, or, failing that, you must accept
        /// last-one-in-wins behavior.
        /// 
        /// Commute allows for more concurrency than just setting the Ref's value
        /// </summary>
        public CommuteRef<A> Commute<X>(X x, Func<X, A, A> f)
        {
            STM.Commute(Id, x, f);
            return new CommuteRef<A>(this);
        }

        /// <summary>
        /// Must be called in a transaction. Sets the in-transaction-value of
        /// ref to:  
        /// 
        ///     `f(in-transaction-value-of-ref)`
        ///     
        /// and returns the in-transaction-value when complete.
        /// 
        /// At the commit point of the transaction, `f` is run *AGAIN* with the
        /// most recently committed value:
        /// 
        ///     `f(most-recently-committed-value-of-ref)`
        /// 
        /// Thus `f` should be commutative, or, failing that, you must accept
        /// last-one-in-wins behavior.
        /// 
        /// Commute allows for more concurrency than just setting the Ref's value
        /// </summary>
        public CommuteRef<A> Commute<X, Y>(X x, Y y, Func<X, Y, A, A> f)
        {
            STM.Commute(Id, x, y, f);
            return new CommuteRef<A>(this);
        }
    }
}
