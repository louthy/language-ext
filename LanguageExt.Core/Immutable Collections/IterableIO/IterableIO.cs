#pragma warning disable CS0660, CS0661
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Collections;
using LanguageExt.Traits;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Lazy sequence
/// </summary>
/// <remarks>
/// Initially designed as a lightweight wrapper around `IEnumerable`, it has since expanded to be a
/// much more powerful lazy sequence of values that supports both `IEnumerable` and `IAsyncEnumerable`. 
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
[CollectionBuilder(typeof(Iterable), nameof(Iterable.create))]
public sealed class IterableIO<A> :
    IEnumerable<A>,
    IAsyncEnumerable<A>,
    Monoid<IterableIO<A>>,
    IAdditiveIdentity<IterableIO<A>, IterableIO<A>>,
    IComparisonOperators<IterableIO<A>, IterableIO<A>, IO<bool>>,
    IAdditionOperators<IterableIO<A>, IterableIO<A>, IterableIO<A>>,
    K<IterableIO, A>
{
    internal readonly IteratorIO<A> iterator;
    
    internal IterableIO(IteratorIO<A> iterator) =>
        this.iterator = iterator;

    /// <summary>
    /// Create an iterable from a span
    /// </summary>
    public static IterableIO<A> FromSpan(ReadOnlySpan<A> ma) =>
        new (IteratorIO.forward(Arr.create(ma)));

    /// <summary>
    /// Empty sequence
    /// </summary>
    public static IterableIO<A> Empty { get; } = 
        new (IteratorIO.empty<A>());

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    [Pure]
    public IO<long> Count =>
        iterator.CountIO;

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IO<IEnumerable<A>> AsEnumerable() =>
        iterator.AsEnumerable();

    /// <summary>
    /// Stream as an async-enumerable
    /// </summary>
    [Pure]
    public IO<IAsyncEnumerable<A>> AsAsyncEnumerable() =>
        iterator.AsAsyncEnumerable();

    /// <summary>
    /// Reverse the sequence
    /// </summary>
    [Pure]
    public IterableIO<A> Reverse() =>
        new (iterator.Reverse());
    
    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public IterableIO<A> Add(A item) =>
        new (iterator.Append(item));

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public IterableIO<A> Cons(A item) =>
        new(IteratorIO.cons(item, iterator));

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    public IO<Unit> Iter(Action<A> f) =>
        iterator.IterIO(f);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    public IO<Unit> Iter(Action<long, A> f) =>
        iterator.IterIO(f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableIO<B> Map<B>(Func<A, B> f) =>
        new(iterator.Map(f));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableIO<B> Map<B>(Func<A, long, B> f, int offset = 0) =>
        new(iterator.Map(f, offset));

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public IterableIO<A> Filter(Func<A, bool> f) =>
        new(iterator.Filter(f));

    /// <summary>
    /// Applies the given function `f` to each element of the sequence. Returns the sequence 
    /// of results for each element where the result is `Some(f(x))`.
    /// </summary>
    /// <param name="f">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public IterableIO<B> Choose<B>(Func<A, Option<B>> f) =>
        new(iterator.Choose(f));

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public IO<bool> Equals<EqA>(IterableIO<A>? other)
        where EqA : Eq<A>  =>
        iterator.Equals<EqA>(other?.iterator);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public IO<bool> Equals(IterableIO<A>? other) =>
        iterator.Equals<EqDefault<A>>(other?.iterator);

    /// <summary>
    /// Semigroup combine two iterables (concatenate)
    /// </summary>
    [Pure]
    public IterableIO<A> Combine(IterableIO<A> items) =>
        new(iterator.Combine(items.iterator));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableIO<A> Concat(IEnumerable<A> items) =>
        new(iterator.Combine(IteratorIO.forward(items)));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableIO<A> Concat(IAsyncEnumerable<A> items) =>
        new(iterator.Combine(IteratorIO.forward(items)));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableIO<A> Concat(IterableIO<A> items) =>
        new(iterator.Combine(items.iterator));

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public IterableIO<A> Distinct<EqA>()
        where EqA : Eq<A> =>
        new(iterator.Distinct<EqA>());

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public IterableIO<A> Distinct() =>
        new(iterator.Distinct());

    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public IO<K<F, IterableIO<B>>> Traverse<F, B>(Func<A, K<F, B>> f)
        where F : Applicative<F>
    {
        return this.FoldIO(add, F.Pure(IterableIO.empty<B>()));
        K<F, IterableIO<B>> add(K<F, IterableIO<B>> state, A value) =>
            Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <remarks>
    /// NOTE: This method will eagerly evaluate the iterable. If you're working with
    /// an asynchronous sequence, then it is advised to use `TraverseIO`.
    /// </remarks>
    /// <param name="f">Mapping function</param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, IterableIO<B>> TraverseM<M, B>(Func<A, K<M, B>> f)
        where M : MonadIO<M>
    {
        return M.LiftIO(this.FoldIO(add, M.Pure(IterableIO.empty<B>()))).Flatten();
        K<M, IterableIO<B>> add(K<M, IterableIO<B>> state, A value) =>
            state.Bind(bs => f(value).Map(bs.Add)); 
    } 

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableIO<B> Bind<B>(Func<A, K<IterableIO, B>> f) =>
        new(iterator.Bind(a => f(a).As().iterator));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableIO<B> Bind<B>(Func<A, IterableIO<B>> f) =>
        new(iterator.Bind(a => f(a).iterator));
    
    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public IO<bool> Any() =>
        iterator.ExistsIO(_ => true);

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public IterableIO<A> Intersperse(A value) =>
        new(iterator.IntersperseIO(value));

    /// <summary>
    /// Consume the item at the head (first) of the sequence or None if the sequence is empty
    /// </summary>
    /// <returns>Optional head item</returns>
    [Pure]
    public IO<Option<A>> Head =>
        iterator.HeadIO;

    /// <summary>
    /// Consume the item at the head (first) of the sequence or `Alternative.Empty` if the sequence is empty
    /// </summary>
    /// <returns>Optional head item</returns>
    [Pure]
    public K<M, A> HeadM<M>()
        where M : MonadIO<M>, Alternative<M> =>
        iterator.HeadM<IteratorIO, M, A>();

    /// <summary>
    /// Consume the first item of the sequence, returning the tail of the sequence. 
    /// </summary>
    /// <returns>The tail items</returns>
    [Pure]
    public IO<IterableIO<A>> Tail =>
        iterator.NextIO() * (n => n is (Exist<A>, var tail)
                                      ? new(tail)
                                      : Empty);

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public IterableIO<A> Skip(long amount) =>
        new(iterator.Skip(amount));

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public IterableIO<A> SkipWhile(Func<A, bool> predicate) =>
        new(iterator.SkipWhile(predicate));

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public IterableIO<A> SkipUntil(Func<A, bool> predicate) =>
        new(iterator.SkipUntil(predicate));
    
    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public IterableIO<A> Take(long amount) =>
        new(iterator.Take(amount));

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public IterableIO<A> TakeWhile(Func<A, bool> predicate) =>
        new(iterator.TakeWhile(predicate));

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public IterableIO<A> TakeUntil(Func<A, bool> predicate) =>
        new(iterator.TakeUntil(predicate));
    
    /// <summary>
    /// Cast items to another type
    /// </summary>
    /// <remarks>
    /// Any item in the sequence that can't be cast to a `B` will be dropped from the result 
    /// </remarks>
    [Pure]
    public IterableIO<B> Cast<B>() =>
        new(iterator.Cast<B>());

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableIO<(A First, B Second)> Zip<B>(IterableIO<B> rhs) =>
        new(iterator.Zip(rhs.iterator));

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableIO<C> Zip<B, C>(IterableIO<B> rhs, Func<A, B, C> zipper) =>
        Zip(rhs).Map(pair => zipper(pair.First, pair.Second));

    /// <summary>
    /// Enumerate the sequence
    /// </summary>
    [Pure]
    public IEnumerator<A> GetEnumerator()
    {
        using var envIO = EnvIO.New();
        foreach (var x in AsEnumerable().Run(envIO))
        {
            yield return x;
        }
    }
    
    /// <summary>
    /// Enumerate the sequence
    /// </summary>
    [Pure]
    public IEnumerator<A> GetEnumerator(CancellationToken cancellationToken)
    {
        using var envIO = EnvIO.New(token: cancellationToken);
        foreach (var x in AsEnumerable().Run(envIO))
        {
            yield return x;
        }
    }
    
    /// <summary>
    /// Enumerate the sequence
    /// </summary>
    [Pure]
    public IEnumerator<A> GetEnumerator(EnvIO envIO)
    {
        foreach (var x in AsEnumerable().Run(envIO))
        {
            yield return x;
        }
    }
    
    /// <summary>
    /// Enumerate the sequence
    /// </summary>
    [Pure]
    public async IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        using var envIO = EnvIO.New(token: cancellationToken);
        await foreach (var x in (await AsAsyncEnumerable().RunAsync(envIO)).WithCancellation(cancellationToken))
        {
            yield return x;
        }
    }
    
    /// <summary>
    /// Enumerate the sequence
    /// </summary>
    [Pure]
    public async IAsyncEnumerator<A> GetAsyncEnumerator(EnvIO envIO)
    {
        await foreach (var x in (await AsAsyncEnumerable().RunAsync(envIO)).WithCancellation(envIO.Token))
        {
            yield return x;
        }
    }

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

    [Pure]
    public IO<int> CompareTo(IterableIO<A>? other) =>
        iterator.CompareTo(other?.iterator);

    [Pure]
    public IO<int> CompareTo<OrdA>(IterableIO<A>? other) 
        where OrdA : Ord<A> =>
        iterator.CompareTo<OrdA>(other?.iterator);

    public static IterableIO<A> AdditiveIdentity { get; } = 
        new (IteratorIO.empty<A>());

    [Pure]
    public static IO<bool> operator ==(IterableIO<A>? lhs, IterableIO<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => IO.pure(true),
            (null, _)    => IO.pure(false),
            (_, null)    => IO.pure(false),
            _            => lhs.iterator == rhs.iterator
        };

    [Pure]
    public static IO<bool> operator !=(IterableIO<A>? lhs, IterableIO<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => IO.pure(false),
            (null, _)    => IO.pure(true),
            (_, null)    => IO.pure(true),
            _            => lhs.iterator != rhs.iterator
        };
    
    [Pure]
    public static IO<bool> operator >(IterableIO<A> left, IterableIO<A> right) => 
        left.iterator > right.iterator;

    [Pure]
    public static IO<bool> operator >=(IterableIO<A> left, IterableIO<A> right) => 
        left.iterator >= right.iterator;

    [Pure]
    public static IO<bool> operator <(IterableIO<A> left, IterableIO<A> right) => 
        left.iterator < right.iterator;

    [Pure]
    public static IO<bool> operator <=(IterableIO<A> left, IterableIO<A> right) =>
        left.iterator <= right.iterator;

    [Pure]
    public static IterableIO<A> operator +(IterableIO<A> left, IterableIO<A> right) => 
        new(left.iterator + right.iterator);

    [Pure]
    public static IterableIO<A> operator +(A left, IterableIO<A> right) => 
        new(right.iterator.Prepend(left));

    [Pure]
    public static IterableIO<A> operator +(IterableIO<A> left, A right) => 
        new(left.iterator.Append(right));
                
    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    public static implicit operator IterableIO<A>(UnitCollection _) =>
        Empty;
    
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableIO<B> Select<B>(Func<A, B> f) =>
        new(iterator.Map(f));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableIO<B> Select<B>(Func<A, long, B> f) =>
        new(iterator.Map(f));

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public IterableIO<A> Where(Func<A, bool> f) =>
        new(iterator.Filter(f));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableIO<B> SelectMany<B>(Func<A, IterableIO<B>> f) =>
        new(iterator.Bind(x => f(x).iterator));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableIO<C> SelectMany<B, C>(Func<A, IterableIO<B>> bind, Func<A, B, C> project) =>
        new(iterator.SelectMany(x => bind(x).iterator, project));

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public new IO<string> ToString() =>
        AsEnumerable().Map(xs => CollectionFormat.ToShortArrayString(xs));

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public IO<string> ToFullString(string separator = ", ") =>
        AsEnumerable().Map(xs => CollectionFormat.ToFullString(xs, separator));
    
    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public IO<string> ToFullArrayString(string separator = ", ") =>
        AsEnumerable().Map(xs => CollectionFormat.ToFullArrayString(xs, separator));
}
