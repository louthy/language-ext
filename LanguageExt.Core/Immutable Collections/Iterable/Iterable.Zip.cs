/*
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

sealed record IterableZip<A, B>(Iterable<A> Left, Iterable<B> Right) : Iterable<(A Left, B Right)>
{
    int? hashCode;

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public override int Count() =>
        Items.Fold(0, (s, iter) => s + iter.Count());

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public override IEnumerable<(A Left, B Right)> AsEnumerable()
    {

    }
    
    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    IAsyncEnumerable<(A Left, B Right)> AsIterable()
    {
        switch(Left.ContainsAsync, Right.ContainsAsync)
        {
            case (true, true):
                return new IterableAsyncEnumerable<(A Left, B Right)>(
                    new AsyncAsyncEnumerable(
                        Left.GetAsyncEnumerable(),
                        Right.GetAsyncEnumerable()));
            
            case (false, false):
                return new IterableEnumerable<(A Left, B Right)>(
                    Left.AsEnumerable().Zip(Right));
            
            case (true, false):
                return new IterableAsyncEnumerable<(A Left, B Right)>(
                    new AsyncSyncEnumerable(
                        Left.GetAsyncEnumerable(),
                        Right));
            
            case (false, true):
                return new IterableAsyncEnumerable<(A Left, B Right)>(
                    new SyncAsyncEnumerable(
                        Left,
                        Right.GetAsyncEnumerable()));
        }
    }

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public override Unit Iter(Action<A> f)
    {
        foreach (var item in this)
        {
            f(item);
        }
        return default;
    }

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public override Unit Iter(Action<A, int> f)
    {
        var ix = 0;
        foreach (var item in this)
        {
            f(item, ix);
            ix++;
        }
        return default;
    }

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Map(f)));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public override Iterable<B> Map<B>(Func<A, int, B> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Map(f)));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public override Iterable<B> Bind<B>(Func<A, Iterable<B>> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Bind(f)));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public override Iterable<B> Bind<B>(Func<A, K<Iterable, B>> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Bind(f)));

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableConcat<A>(Items.Map(xs => xs.Filter(f)));

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public override Iterable<A> Intersperse(A value) =>
        new IterableConcat<A>(Items.Map(xs => xs.Intersperse(value)));

    [Pure]
    public override int CompareTo(Iterable<A>? obj) =>
        CompareTo<OrdDefault<A>>(obj);

    [Pure]
    public override int CompareTo<OrdA>(Iterable<A>? obj)
    {
        if (obj is null) return 1;
        using var iterA = AsEnumerable().GetEnumerator();
        using var iterB = obj.AsEnumerable().GetEnumerator();
        while (true)
        {
            var movedA = iterA.MoveNext();
            var movedB = iterA.MoveNext();
            switch (movedA, movedB)
            {
                case (true, true):
                    var cmp = OrdA.Compare(iterA.Current, iterB.Current);
                    if (cmp != 0) return cmp;
                    break;
                
                case (false, true):
                    return -1;
                
                case (true, false):
                    return 1;
                
                case (false, false):
                    return 0;
            }
        }
    }

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(SafeIterable());

    /// <summary>
    /// Converts all elements to strings, but doesn't evaluate the async sequences, instead, injects
    /// an 'async sequence...' string.
    /// </summary>
    Iterable<string> SafeIterable() =>
        Items.AsIterable()
             .Bind(items => items switch
                            {
                                IterableAsyncEnumerable<A> => ["async sequence..."],
                                IterableEnumerable<A> xs   => xs.Map(x => x?.ToString() ?? "[null]"),
                                IterableConcat<A> xs       => xs.SafeIterable(),
                                _                          => throw new NotSupportedException()
                            });

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public override string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(this);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public override string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(this);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public override bool Equals<EqA>(Iterable<A> rhs) where EqA : Eq<A>;

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public override bool Equals(Iterable<A>? other);

    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    public override Iterable<A> Skip(int amount);

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    public override Iterable<A> Take(int amount); 

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public override Iterable<A> TakeWhile(Func<A, bool> pred);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't.  An index value is 
    /// also provided to the predicate function.
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public override Iterable<A> TakeWhile(Func<A, int, bool> pred);

    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    [Pure]
    public override (Iterable<A> First, Iterable<A> Second) Partition(Func<A, bool> predicate);

    /// <summary>
    /// Cast items to another type
    /// </summary>
    /// <remarks>
    /// Any item in the sequence that can't be cast to a `B` will be dropped from the result 
    /// </remarks>
    [Pure]
    public override Iterable<B> Cast<B>();

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public override  Iterable<(A First, B Second)> Zip<B>(Iterable<B> rhs);

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public override Iterable<C> Zip<B, C>(Iterable<B> rhs, Func<A, B, C> zipper);    

    /// <summary>
    /// Empty sequence
    /// </summary>
    [Pure]
    public static Iterable<A> Empty => 
        new IterableEnumerable<A>(Enumerable.Empty<A>());

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(Iterable<A> x, Iterable<A> y) =>
        x.ConcatFast(y);

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
    /// Enumerator  
    /// </summary>
    public override IEnumerator<A> GetEnumerator();

    class AsyncAsyncEnumerable(IAsyncEnumerable<A> Left, IAsyncEnumerable<B> Right) : IAsyncEnumerable<(A, B)>
    {
        public IAsyncEnumerator<(A, B)> GetAsyncEnumerator(CancellationToken token = default) =>
            new Enumerator(Left.GetAsyncEnumerator(token), Right.GetAsyncEnumerator(token), token);


        class Enumerator(IAsyncEnumerator<A> Left, IAsyncEnumerator<B> Right, CancellationToken Token) : IAsyncEnumerator<(A, B)>
        {
            public int Order;
            
            public ValueTask<bool> MoveNextAsync()
            {
                var      ltask = Left.MoveNextAsync(); 
                var      rtask = Right.MoveNextAsync();
                SpinWait sw    = default;
                while (!ltask.IsCompleted || !rtask.IsCompleted)
                {
                    if(Token.IsCancellationRequested) return ValueTask.FromCanceled<bool>(Token);
                    sw.SpinOnce();
                }
                switch (ltask.Result, rtask.Result)
                {
                    case (true, true): 
                        return ValueTask.FromResult(true);
                    
                    case (false, false): 
                        return ValueTask.FromResult(false);
                    
                    case (false, true):
                        Order = -1;
                        return ValueTask.FromResult(false);
                    
                    case (true, false):
                        Order = 1;
                        return ValueTask.FromResult(false);
                }
            }

            public (A, B) Current =>
                (Left.Current, Right.Current);

            public async ValueTask DisposeAsync()
            {
                await Left.DisposeAsync().ConfigureAwait(false);
                await Right.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    class AsyncSyncEnumerable(IAsyncEnumerable<A> Left, IEnumerable<B> Right) : IAsyncEnumerable<(A, B)>
    {
        public IAsyncEnumerator<(A, B)> GetAsyncEnumerator(CancellationToken token = new CancellationToken()) =>
            new Enumerator(Left.GetAsyncEnumerator(token), Right.GetEnumerator(), token);

        class Enumerator(IAsyncEnumerator<A> Left, IEnumerator<B> Right, CancellationToken Token) : IAsyncEnumerator<(A, B)>
        {
            public int Order;
            
            public ValueTask<bool> MoveNextAsync()
            {
                var      ltask = Left.MoveNextAsync(); 
                var      rtask = Right.MoveNext();
                SpinWait sw    = default;
                while (!ltask.IsCompleted)
                {
                    if(Token.IsCancellationRequested) return ValueTask.FromCanceled<bool>(Token);
                    sw.SpinOnce();
                }
                switch (ltask.Result, rtask)
                {
                    case (true, true): 
                        return ValueTask.FromResult(true);
                    
                    case (false, false): 
                        return ValueTask.FromResult(false);
                    
                    case (false, true):
                        Order = -1;
                        return ValueTask.FromResult(false);
                    
                    case (true, false):
                        Order = 1;
                        return ValueTask.FromResult(false);
                }
            }

            public (A, B) Current =>
                (Left.Current, Right.Current);

            public async ValueTask DisposeAsync()
            {
                await Left.DisposeAsync().ConfigureAwait(false);
                Right.Dispose();
            }
        }
    }

    class SyncAsyncEnumerable(IEnumerable<A> Left, IAsyncEnumerable<B> Right) : IAsyncEnumerable<(A, B)>
    {
        public IAsyncEnumerator<(A, B)> GetAsyncEnumerator(CancellationToken token = new CancellationToken()) =>
            new Enumerator(Left.GetEnumerator(), Right.GetAsyncEnumerator(token), token);

        class Enumerator(IEnumerator<A> Left, IAsyncEnumerator<B> Right, CancellationToken Token) : IAsyncEnumerator<(A, B)>
        {
            public int Order;

            public ValueTask<bool> MoveNextAsync()
            {
                var      ltask = Left.MoveNext(); 
                var      rtask = Right.MoveNextAsync();
                SpinWait sw    = default;
                while (!rtask.IsCompleted)
                {
                    if(Token.IsCancellationRequested) return ValueTask.FromCanceled<bool>(Token);
                    sw.SpinOnce();
                }

                switch (ltask, rtask.Result)
                {
                    case (true, true): 
                        return ValueTask.FromResult(true);
                    
                    case (false, false): 
                        return ValueTask.FromResult(false);
                    
                    case (false, true):
                        Order = -1;
                        return ValueTask.FromResult(false);
                    
                    case (true, false):
                        Order = 1;
                        return ValueTask.FromResult(false);
                }
            }

            public (A, B) Current =>
                (Left.Current, Right.Current);

            public async ValueTask DisposeAsync()
            {
                Left.Dispose();
                await Right.DisposeAsync().ConfigureAwait(false);
            }
        }
    }    
}
*/
