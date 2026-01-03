using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Lazy sequence
/// </summary>
/// <remarks>
/// This is a lightweight wrapper around `IEnumerable` that also implements traits
/// that make it play nice with other types in this library: Monad, Traversable, etc. 
/// </remarks>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
[CollectionBuilder(typeof(Iterable), nameof(Iterable.createRange))]
public abstract class Iterable<A> :
    IEnumerable<A>,
    IAsyncEnumerable<A>,
    Monoid<Iterable<A>>,
    IComparable<Iterable<A>>,
    IAdditiveIdentity<Iterable<A>, Iterable<A>>,
    IComparisonOperators<Iterable<A>, Iterable<A>, bool>,
    IAdditionOperators<Iterable<A>, Iterable<A>, Iterable<A>>,
    K<Iterable, A>
{
    int? hashCode;

    /// <summary>
    /// True if this iterable or any component part of the structure has asynchonicity.  
    /// </summary>
    internal abstract bool IsAsync { get; }

    /// <summary>
    /// Create an iterable from a span
    /// </summary>
    public static Iterable<A> FromSpan(ReadOnlySpan<A> ma) =>
        new IterableEnumerable<A>(IO.pure<IEnumerable<A>>(ma.ToArray()));

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public int Count() =>
        CountIO().Run();

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    [Pure]
    public abstract IO<int> CountIO();

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable(CancellationToken token = default)
    {
        using var env = EnvIO.New(token: token);
        var       xs  = AsEnumerableIO().Run();
        foreach (var x in xs)
        {
            if(env.Token.IsCancellationRequested) throw new OperationCanceledException();
            yield return x;
        }
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public abstract IO<IEnumerable<A>> AsEnumerableIO();

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public async IAsyncEnumerable<A> AsAsyncEnumerable([EnumeratorCancellation] CancellationToken token = default)
    {
        using var env = EnvIO.New(token: token);
        var xs = await AsAsyncEnumerableIO().RunAsync(env);
        await foreach (var x in xs.WithCancellation(token))
        {
            yield return x;
        }
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public abstract IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO();

    /// <summary>
    /// Reverse the sequence
    /// </summary>
    [Pure]
    public abstract Iterable<A> Reverse();

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public virtual Iterable<A> Add(A item) =>
        new IterableAdd<A>(
            new SeqStrict<A>(new A[8], 8, 0, 0, 0),
            this,
            new SeqStrict<A>([item, default!, default!, default!, default!, default!, default!, default!], 0, 1, 0, 0)); 

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public virtual Iterable<A> Cons(A item) =>
        new IterableAdd<A>(
            new SeqStrict<A>([default!, default!, default!, default!, default!, default!, default!, item], 7, 1, 0, 0),
            this,
            new SeqStrict<A>(new A[8], 0, 0, 0, 0));

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<A> f) =>
        IterIO(f).Run();

    /// <summary>
    /// Pure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    [Pure]
    public IO<Unit> IterIO(Action<A> f) =>
        IsAsync
            ? IO.liftVAsync(async env =>
                            {
                                await foreach (var x in AsAsyncEnumerable(env.Token))
                                {
                                    f(x);
                                }
                                return unit;
                            })
            : IO.lift(env =>
                      {
                          foreach (var x in AsEnumerable(env.Token))
                          {
                              f(x);
                          }
                          return unit;
                      });


    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<A, int> f) =>
        IterIO(f).Run();

    /// <summary>
    /// Pure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    [Pure]
    public IO<Unit> IterIO(Action<A, int> f, int offset = 0) =>
        IsAsync
            ? IO.liftVAsync(async env =>
                            {
                                var ix = offset;
                                await foreach (var x in AsAsyncEnumerable(env.Token))
                                {
                                    f(x, ix++);
                                }

                                return unit;
                            })
            : IO.lift(env =>
                      {
                          var ix = offset;
                          foreach (var x in AsEnumerable(env.Token))
                          {
                              f(x, ix++);
                          }

                          return unit;
                      });
    
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public abstract Iterable<B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Map<B>(Func<A, int, B> f, int offset = 0) =>
        Zip(Iterable.createRange(Enumerable.InfiniteSequence(offset, 1)))
           .Map(p => f(p.First, p.Second));

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public abstract Iterable<A> Filter(Func<A, bool> f);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public IO<bool> EqualsIO<EqA>(Iterable<A>? other) 
        where EqA : Eq<A> =>
        other is null
            ? IO.pure(false)
            : ReferenceEquals(this, other)
                  ? IO.pure(true)
                  : (IsAsync, IsAsync) switch
                    {
                        (true, true)   => IO.liftVAsync(async env => await AsAsyncEnumerable(env.Token).SequenceEqualAsync(other.AsAsyncEnumerable(env.Token), Eq.Comparer<EqA, A>())),
                        (true, false)  => IO.liftVAsync(async env => await AsAsyncEnumerable(env.Token).SequenceEqualAsync(other.AsAsyncEnumerable(env.Token), Eq.Comparer<EqA, A>())),
                        (false, true)  => IO.liftVAsync(async env => await AsAsyncEnumerable(env.Token).SequenceEqualAsync(other.AsAsyncEnumerable(env.Token), Eq.Comparer<EqA, A>())),
                        (false, false) => IO.lift(env => AsEnumerable(env.Token).SequenceEqual(other.AsEnumerable(env.Token), Eq.Comparer<EqA, A>()))
                    };
    
    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public IO<bool> EqualsIO(Iterable<A>? other) =>
        EqualsIO<EqDefault<A>>(other);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(Iterable<A>? other)
        where EqA : Eq<A> =>
        EqualsIO<EqA>(other).Run();

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals(Iterable<A>? other) =>
        EqualsIO<EqDefault<A>>(other).Run();

    [Pure]
    public IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken cancellationToken = default) => 
        AsAsyncEnumerable(cancellationToken).GetAsyncEnumerator(cancellationToken);

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Iterable<A> rhs && Equals(rhs);

    [Pure]
    public static bool operator ==(Iterable<A>? lhs, Iterable<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => true,
            (null, _)    => false,
            (_, null)    => false,
            _            => lhs.Equals(rhs)
        };

    [Pure]
    public static bool operator !=(Iterable<A>? lhs, Iterable<A>? rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Semigroup combine two iterables (concatenate)
    /// </summary>
    [Pure]
    public Iterable<A> Combine(Iterable<A> y) =>
        Concat(y);

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(IEnumerable<A> items) =>
        Concat(new IterableEnumerable<A>(IO.pure(items)));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(IAsyncEnumerable<A> items) =>
        Concat(new IterableAsyncEnumerable<A>(IO.pure(items)));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(Iterable<A> items) =>
        (this, items) switch
        {
            (IterableConcat<A> l, IterableConcat<A> r) => new IterableConcat<A>(l.Items + r.Items),
            (IterableConcat<A> l, var r)               => new IterableConcat<A>(l.Items.Add(r)),
            (var l, IterableConcat<A> r)               => new IterableConcat<A>(l.Cons(r.Items)),
            var (l, r)                                 => new IterableConcat<A>(Seq(l, r))
        };

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public Iterable<A> Distinct<EqA>()
        where EqA : Eq<A> =>
        IsAsync
            ? new IterableAsyncEnumerable<A>(AsAsyncEnumerableIO().Map(xs => xs.Distinct(Eq.Comparer<EqA, A>())))
            : new IterableEnumerable<A>(AsEnumerableIO().Map(xs => xs.Distinct(Eq.Comparer<EqA, A>())));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public Iterable<A> Distinct() =>
        Distinct<EqDefault<A>>();

    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <remarks>
    /// NOTE: This method will eagerly evaluate the iterable. If you're working with
    /// an asynchronous sequence, then it is advised to use `TraverseIO`.
    /// </remarks>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Iterable<B>> Traverse<F, B>(Func<A, K<F, B>> f)
        where F : Applicative<F>
    {
        return FoldIO(add, F.Pure(Iterable<B>.Empty)).Run();
        K<F, Iterable<B>> add(K<F, Iterable<B>> state, A value) =>
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
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, Iterable<B>> TraverseM<M, B>(Func<A, K<M, B>> f)
        where M : Monad<M>
    {
        return FoldIO(add, M.Pure(Iterable<B>.Empty)).Run();
        K<M, Iterable<B>> add(K<M, Iterable<B>> state, A value) =>
            state.Bind(bs => f(value).Map(bs.Add)); 
    }    

    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Iterable<B>> TraverseIO<F, B>(Func<A, K<F, B>> f, K<Iterable, A> kta) 
        where F : MonadIO<F>
    {
        var ta = +kta;
        return F.LiftIO(ta.FoldIO(add, F.Pure(Iterable<B>.Empty))).Flatten();

        K<F, Iterable<B>> add(K<F, Iterable<B>> state, A value) =>
            state.Bind(bs => f(value).Bind(b => F.Pure(bs.Add(b)))); 
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> Bind<B>(Func<A, K<Iterable, B>> f)
    {
        return IsAsync
                   ? new IterableAsyncEnumerable<B>(IO.lift(async))
                   : new IterableAsyncEnumerable<B>(IO.lift(sync));

        async IAsyncEnumerable<B> async(EnvIO env)
        {
            await foreach (var a in AsAsyncEnumerable(env.Token))
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                var mb = +f(a);
                if (mb.IsAsync)
                {
                    await foreach (var b in mb.AsAsyncEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
                else
                {
                    foreach (var b in mb.AsEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
            }
        }
        async IAsyncEnumerable<B> sync(EnvIO env)
        {
            foreach (var a in AsEnumerable(env.Token))
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                var mb = +f(a);
                if (mb.IsAsync)
                {
                    await foreach (var b in mb.AsAsyncEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
                else
                {
                    foreach (var b in mb.AsEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> Bind<B>(Func<A, Iterable<B>> f)
    {
        return IsAsync
                   ? new IterableAsyncEnumerable<B>(IO.lift(async))
                   : new IterableAsyncEnumerable<B>(IO.lift(sync));

        async IAsyncEnumerable<B> async(EnvIO env)
        {
            await foreach (var a in AsAsyncEnumerable(env.Token))
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                var mb = f(a);
                if (mb.IsAsync)
                {
                    await foreach (var b in mb.AsAsyncEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
                else
                {
                    foreach (var b in mb.AsEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
            }
        }

        async IAsyncEnumerable<B> sync(EnvIO env)
        {
            foreach (var a in AsEnumerable(env.Token))
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                var mb = f(a);
                if (mb.IsAsync)
                {
                    await foreach (var b in mb.AsAsyncEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
                else
                {
                    foreach (var b in mb.AsEnumerable(env.Token))
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        yield return b;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public S FoldWhile<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        FoldWhileIO(f, predicate, initialState).Run();

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public abstract IO<S> FoldWhileIO<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState);

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public S FoldUntil<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        FoldUntilIO(f, predicate, initialState).Run();

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public abstract IO<S> FoldUntilIO<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState);

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public S FoldMaybe<S>(
        Func<S, A, Option<S>> f,
        S initialState) =>
        FoldMaybeIO(f, initialState).Run();

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public IO<S> FoldMaybeIO<S>(
        Func<S, A, Option<S>> f,
        S initialState) =>
        FoldWhileIO<(bool IsSome, S Value)>(
                (s, a) => f(s.Value, a) switch
                          {
                              { IsSome: true, Case: S value } => (true, value),
                              _                               => (false, s.Value)
                          },
                s => s.State.IsSome,
                (true, initialState))
           .Map(s => s.Value);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public K<M, S> FoldWhileM<M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldWhileIO(acc, s => predicate(s.Value), Monad.pure<M, S>)
           .Bind(f1 => f1(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(state, value), bind);
    }

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public K<M, S> FoldUntilM<M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldUntilIO(acc, s => predicate(s.Value), Monad.pure<M, S>).Bind(f1 => f1(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(value)(state), bind);
    }

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public K<M, S> FoldUntilM<M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldUntilIO(acc, s => predicate(s.Value), Monad.pure<M, S>).Bind(f1 => f1(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(state, value), bind);
    }

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public S Fold<S>(Func<S, A, S> f, S initialState) =>
        FoldIO(f, initialState).Run();

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public IO<S> FoldIO<S>(Func<S, A, S> f, S initialState) =>
        FoldWhileIO(f, _ => true, initialState);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public K<M, S> FoldM<M, S>(
        Func<S, A, K<M, S>> f, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldIO(acc, Monad.pure<M, S>).Bind(f => f(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(state, value), bind);
    }
    
    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public bool Any() =>
        this.Exists(_ => true);

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public Iterable<A> Intersperse(A value)
    {
        return IsAsync
                   ? new IterableAsyncEnumerable<A>(IO.lift(goAsync))
                   : new IterableEnumerable<A>(IO.lift(goSync));
        
        IEnumerable<A> goSync(EnvIO env)
        {
            var isFirst = true;
            foreach(var item in AsEnumerable(env.Token))
            {
                if (!isFirst)
                {
                    yield return value;
                }

                yield return item;
                isFirst = false;
            }
        }
        
        async IAsyncEnumerable<A> goAsync(EnvIO env)
        {
            {
                var isFirst = true;
                await foreach(var item in AsAsyncEnumerable(env.Token))
                {
                    if (!isFirst)
                    {
                        yield return value;
                    }

                    yield return item;
                    isFirst = false;
                }
            }
        }
    }

    [Pure]
    public IO<int> CompareToIO(object? obj) =>
        obj is Iterable<A> rhs
            ? CompareToIO(rhs)
            : IO.pure(1);

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Iterable<A> rhs
            ? CompareTo(rhs)
            : 1;

    [Pure]
    public IO<int> CompareToIO(Iterable<A>? other) =>
        CompareToIO<OrdDefault<A>>(other);

    [Pure]
    public int CompareTo(Iterable<A>? other) =>
        CompareTo<OrdDefault<A>>(other);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public IO<int> CompareToIO<OrdA>(Iterable<A>? rhs) 
        where OrdA : Ord<A>
    {
        if (rhs is null) return IO.pure(1);

        return IsAsync || rhs.IsAsync
                   ? IO.liftVAsync(async env =>
                                   {
                                       var iterA = GetAsyncEnumerator(env.Token);
                                       var iterB = rhs.GetAsyncEnumerator(env.Token);
                                       while (await iterA.MoveNextAsync())
                                       {
                                           if (await iterB.MoveNextAsync())
                                           {
                                               var cmp = OrdA.Compare(iterA.Current, iterB.Current);
                                               if (cmp != 0) return cmp;
                                           }
                                           else
                                           {
                                               return 1;
                                           }
                                       }

                                       if (await iterB.MoveNextAsync())
                                       {
                                           return -1;
                                       }

                                       return 0;
                                   })
                   : IO.lift(env =>
                             {
                                 using var iterA = AsEnumerable(env.Token).GetEnumerator();
                                 using var iterB = rhs.AsEnumerable(env.Token).GetEnumerator();
                                 while (iterA.MoveNext())
                                 {
                                     if (iterB.MoveNext())
                                     {
                                         var cmp = OrdA.Compare(iterA.Current, iterB.Current);
                                         if (cmp != 0) return cmp;
                                     }
                                     else
                                     {
                                         return 1;
                                     }
                                 }

                                 if (iterB.MoveNext())
                                 {
                                     return -1;
                                 }

                                 return 0;
                             });
    }

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(Iterable<A>? rhs) 
        where OrdA : Ord<A> =>
        CompareToIO<OrdA>(rhs).Run();

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        ToStringIO().Run();

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public virtual IO<string> ToStringIO() =>
        // TODO: Replace with FoldIO
        AsEnumerableIO().Map(xs => CollectionFormat.ToShortArrayString(xs));

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        ToFullStringIO(separator).Run();

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public virtual IO<string> ToFullStringIO(string separator = ", ") =>
        // TODO: Replace with FoldIO
        AsEnumerableIO().Map(xs => CollectionFormat.ToFullString(xs, separator));

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        ToFullArrayStringIO(separator).Run();

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public virtual IO<string> ToFullArrayStringIO(string separator = ", ") =>
        // TODO: Replace with FoldIO
        AsEnumerableIO().Map(xs => CollectionFormat.ToFullArrayString(AsEnumerable(), separator));

    /// <summary>
    /// Tail of the iterable
    /// </summary>
    public Iterable<A> Tail =>
        Skip(1);

    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    public Iterable<A> Skip(int amount) =>
        amount < 1
            ? this
            : IsAsync
                ? new IterableEnumerable<A>(AsEnumerableIO().Map(xs => xs.Skip(amount)))
                : new IterableAsyncEnumerable<A>(AsAsyncEnumerableIO().Map(xs => xs.Skip(amount)));

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    public Iterable<A> Take(int amount) =>
        amount < 1
            ? Empty
            : IsAsync
                ? new IterableEnumerable<A>(AsEnumerableIO().Map(xs => xs.Take(amount)))
                : new IterableAsyncEnumerable<A>(AsAsyncEnumerableIO().Map(xs => xs.Take(amount)));

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, bool> pred)
    {
        return IsAsync
                   ? new IterableAsyncEnumerable<A>(IO.lift(async))
                   : new IterableEnumerable<A>(IO.lift(sync));
            
        IEnumerable<A> sync(EnvIO env)
        {
            foreach (var x in AsEnumerable(env.Token))
            {
                if (!pred(x)) break;
                yield return x;
            }
        }
            
        async IAsyncEnumerable<A> async(EnvIO env)
        {
            await foreach (var x in AsAsyncEnumerable(env.Token))
            {
                if (!pred(x)) break;
                yield return x;
            }
        }        
    }

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided; and stopping as soon as one doesn't.  An index value is 
    /// also provided to the predicate function.
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, int, bool> pred)
    {
        return IsAsync
                   ? new IterableAsyncEnumerable<A>(IO.lift(async))
                   : new IterableEnumerable<A>(IO.lift(sync));
            
        IEnumerable<A> sync(EnvIO env)
        {
            var ix = 0;
            foreach (var x in AsEnumerable(env.Token))
            {
                if (!pred(x, ix++)) break;
                yield return x;
            }
        }
            
        async IAsyncEnumerable<A> async(EnvIO env)
        {
            var ix = 0;
            await foreach (var x in AsAsyncEnumerable(env.Token))
            {
                if (!pred(x, ix++)) break;
                yield return x;
            }
        }        
    }

    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    [Pure]
    public IO<(Iterable<A> First, Iterable<A> Second)> PartitionIO(Func<A, bool> predicate)
    {
        return IsAsync
                   ? IO.liftVAsync(async)
                   : IO.lift(sync);
        
        async ValueTask<(Iterable<A> First, Iterable<A> Second)> async(EnvIO env)
        {
            Iterable<A> f = new IterableStrict<A>(SeqStrict<A>.Empty);
            Iterable<A> s = new IterableStrict<A>(SeqStrict<A>.Empty);

            await foreach (var item in AsAsyncEnumerable(env.Token))
            {
                if (predicate(item))
                {
                    f = f.Add(item);
                }
                else
                {
                    s = s.Add(item);
                }
            }
            return (f, s);
        }
 
        (Iterable<A> First, Iterable<A> Second) sync(EnvIO env)
        {
            Iterable<A> f = new IterableStrict<A>(SeqStrict<A>.Empty);
            Iterable<A> s = new IterableStrict<A>(SeqStrict<A>.Empty);

            foreach (var item in AsEnumerable(env.Token))
            {
                if (predicate(item))
                {
                    f = f.Add(item);
                }
                else
                {
                    s = s.Add(item);
                }
            }

            return (f, s);
        }
    }

    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    [Pure]
    public (Iterable<A> First, Iterable<A> Second) Partition(Func<A, bool> predicate) =>
        PartitionIO(predicate).Run();
    
    /// <summary>
    /// Cast items to another type
    /// </summary>
    /// <remarks>
    /// Any item in the sequence that can't be cast to a `B` will be dropped from the result 
    /// </remarks>
    [Pure]
    public Iterable<B> Cast<B>() =>
        new IterableCast<A, B>(this);

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public Iterable<(A First, B Second)> Zip<B>(Iterable<B> rhs) =>
        new IterableZip<A, B>(this, rhs);

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public Iterable<C> Zip<B, C>(Iterable<B> rhs, Func<A, B, C> zipper) =>
        Zip(rhs).Map(pair => zipper(pair.First, pair.Second));


    /// <summary>
    /// Empty sequence
    /// </summary>
    [Pure]
    public static Iterable<A> Empty => 
        IterableNil<A>.Default;

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(Iterable<A> x, Iterable<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(A x, Iterable<A> y) =>
        x.Cons(y);

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(Iterable<A> x, A y) =>
        x.Add(y);

    /// <summary>
    /// If this sequence is empty, return the other sequence, otherwise return this sequence.
    /// </summary>
    /// <param name="rhs">Right hand side of the operator</param>
    /// <returns>A choice between two sequences based</returns>
    [Pure]
    public abstract Iterable<A> Choose(Iterable<A> rhs);

    /// <summary>
    /// If this sequence is empty, return the other sequence, otherwise return this sequence.
    /// </summary>
    /// <param name="rhs">Right hand side of the operator</param>
    /// <returns>A choice between two sequences based</returns>
    [Pure]
    public abstract Iterable<A> Choose(Memo<Iterable, A> rhs);
    
    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> operator |(Iterable<A> x, K<Iterable, A> y) =>
        x.Choose(+y);

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> operator |(K<Iterable, A> x, Iterable<A> y) =>
        x.As().Choose(y);
    
    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Iterable<A> operator |(Iterable<A> x, Memo<Iterable, A> y) =>
        x.Choose(y);

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >=(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <=(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) <= 0;
                
    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    public static implicit operator Iterable<A>(SeqEmpty _) =>
        Empty;

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Select<B>(Func<A, int, B> f) =>
        Map(f);

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public Iterable<A> Where(Func<A, bool> f) =>
        Filter(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, Iterable<B>> bind) =>
        Bind(bind);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, Iterable<B>> bind, Func<A, B, C> project) =>
        Bind(x  => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, IEnumerable<B>> bind) =>
        Bind(x => bind(x).AsIterable());

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, IEnumerable<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).AsIterable().Map(y => project(x, y)));

    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        AsEnumerable().GetEnumerator();

    /// <summary>
    /// Get the hash code for all the items in the sequence, or 0 if empty
    /// </summary>
    /// <returns></returns>
    [Pure]
    public override int GetHashCode() =>
        hashCode is null
            ? (hashCode = hash(AsEnumerable())).Value
            : hashCode.Value;

    /// <summary>
    /// Get the additive-identity, i.e. the monoid-zero.  Which is the empty sequence/
    /// </summary>
    public static Iterable<A> AdditiveIdentity => 
        Empty;
    
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();
}
