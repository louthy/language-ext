using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt;

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
    public event AtomChangedEvent<A>? Change; 

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
    /// Value accessor (read and write)
    /// </summary>
    public A Value
    {
        get => (A)STM.Read(Id);
        set => STM.Write(Id, value!);
    }

    /// <summary>
    /// Value accessor (read and write)
    /// </summary>
    /// <remarks>
    /// 
    /// * Gets will return a freshly constructed `IO` monad that can be repeatedly
    /// evaluated to get the latest state of the `Ref`.
    /// 
    /// * Sets pass an `IO` monad that will be mapped to an operation that will set
    /// the value of the `Ref` each time it's evaluated.
    /// 
    /// </remarks>
    public IO<A> ValueIO
    {
        get => IO.lift(_ => (A)STM.Read(Id));
        set => value.Map(v =>
                         {
                             STM.Write(Id, v!);
                             return unit;
                         });
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
    public override bool Equals(object? obj) =>
        obj is A val && Equals(val);

    /// <summary>
    /// Equality
    /// </summary>
    public bool Equals(A? other) =>
        EqDefault<A?>.Equals(other, Value);

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
    public IO<A> SwapIO(Func<A, A> f) =>
        IO.lift(_ =>
        {
            var fv = f(Value);
            Value = fv;
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
}
