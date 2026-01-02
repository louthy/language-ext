using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Channels;
using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
namespace LanguageExt;

public partial class SourceT
{
    /// <summary>
    /// Empty source
    /// </summary>
    /// <remarks>
    /// This is a 'void' source, it yields zero values. 
    /// </remarks>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Uninhabited source</returns>
    [Pure]
    public static SourceT<M, A> empty<M, A>() 
        where M : MonadIO<M> =>
        EmptySourceT<M, A>.Default;
    
    /// <summary>
    /// Lift a pure value into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one value. 
    /// </remarks>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    [Pure]
    public static SourceT<M, A> pure<M, A>(A value) 
        where M : MonadIO<M> =>
        new PureSourceT<M, A>(value);

    /// <summary>
    /// Indicate the stream is complete
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    public static SourceT<M, Unit> done<M>() 
        where M : MonadIO<M> =>
        new DoneSourceT<M>();
    
    /// <summary>
    /// Lift a foldable of pure values into a `SourceT`
    /// </summary>
    /// <param name="fa">Foldable of pure values</param>
    /// <typeparam name="F">Foldable trait type</typeparam>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`SourceT`</returns>
    [Pure]
    public static SourceT<M, A> liftFoldable<F, M, A>(K<F, A> fa)
        where M : MonadIO<M>
        where F : Foldable<F> =>
        new FoldablePureSourceT<F, M, A>(fa);

    /// <summary>
    /// Lift a foldable of monadic values into a `SourceT`
    /// </summary>
    /// <param name="fma">Foldable of monadic values</param>
    /// <typeparam name="F">Foldable trait type</typeparam>
    /// <typeparam name="M">Monad trait type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>`SourceT`</returns>
    [Pure]
    public static SourceT<M, A> liftFoldableM<F, M, A>(K<F, K<M, A>> fma)
        where M : MonadIO<M>
        where F : Foldable<F> =>
        new FoldableSourceT<F, M, A>(fma);
    
    /// <summary>
    /// Lift a structure into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one structure. 
    /// </remarks>
    /// <param name="ma">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    [Pure]
    public static SourceT<M, A> liftM<M, A>(K<M, A> ma) 
        where M : MonadIO<M> =>
        new LiftSourceT<M, A>(ma);
    
    /// <summary>
    /// Lift a structure into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one structure. 
    /// </remarks>
    /// <param name="ma">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    [Pure]
    public static SourceT<M, A> liftIO<M, A>(K<IO, A> ma) 
        where M : MonadIO<M> =>
        new LiftSourceT<M, A>(M.LiftIO(ma));
    
    /// <summary>
    /// Lift a pure value into the source and yield it for infinity
    /// </summary>
    /// <remarks>
    /// This is an infinite source, it repeatedly yields a value. 
    /// </remarks>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Infinite source</returns>
    [Pure]
    public static SourceT<M, A> forever<M, A>(A value) 
        where M : MonadIO<M> =>
        new ForeverSourceT<M, A>(M.Pure(value));
    
    /// <summary>
    /// Lift a structure into the source and yield it for infinity
    /// </summary>
    /// <remarks>
    /// This is an infinite source, it repeatedly yields the provided structure. 
    /// </remarks>
    /// <param name="ma">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Infinite source</returns>
    [Pure]
    public static SourceT<M, A> foreverM<M, A>(K<M, A> ma) 
        where M : MonadIO<M> =>
        new ForeverSourceT<M, A>(ma);

    /// <summary>
    /// Make a `System.Threading.Channels.Channel` into a source of values
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> lift<M, A>(Channel<A> channel) 
        where M : MonadIO<M>, Fallible<M> =>
        new MultiListenerPureSourceT<M, A>(channel);

    /// <summary>
    /// Make a `System.Threading.Channels.Channel` into a source of values
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> liftM<M, A>(Channel<K<M, A>> channel) 
        where M : MonadIO<M>, Fallible<M> =>
        new MultiListenerSourceT<M, A>(channel);

    /// <summary>
    /// Make a `Source` into a `SourceT`
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> lift<M, A>(Source<A> channel) 
        where M : MonadIO<M> =>
        new SourcePureSourceT<M, A>(channel);

    /// <summary>
    /// Make a `Source` into a `SourceT`
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> liftM<M, A>(Source<K<M, A>> channel) 
        where M : MonadIO<M> =>
        new SourceSourceT<M, A>(channel);

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> lift<M, A>(IEnumerable<A> items) 
        where M : MonadIO<M> =>
        new IteratorSyncSourceT<M, A>(items.Select(M.Pure));

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> liftM<M, A>(IEnumerable<K<M, A>> items) 
        where M : MonadIO<M> =>
        new IteratorSyncSourceT<M, A>(items);

    /// <summary>
    /// Make an `IObservable` into a source of values
    /// </summary>
    /// <param name="items">`IObservable` to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> lift<M, A>(IObservable<A> items) 
        where M : MonadIO<M> =>
        new ObservablePureSourceT<M, A>(items);

    /// <summary>
    /// Make an `IObservable` into a source of values
    /// </summary>
    /// <param name="items">`IObservable` to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> liftM<M, A>(IObservable<K<M, A>> items) 
        where M : MonadIO<M> =>
        new ObservableSourceT<M, A>(items);

    /// <summary>
    /// Make an `IAsyncEnumerable` into a source of values
    /// </summary>
    /// <param name="items">`IAsyncEnumerable` to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> lift<M, A>(IAsyncEnumerable<A> items) 
        where M : MonadIO<M> =>
        new IteratorAsyncSourceT<M, A>(items.Select(M.Pure));

    /// <summary>
    /// Make an `IAsyncEnumerable` into a source of values
    /// </summary>
    /// <param name="items">`IAsyncEnumerable` to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    [Pure]
    public static SourceT<M, A> liftM<M, A>(IAsyncEnumerable<K<M, A>> items) 
        where M : MonadIO<M> =>
        new IteratorAsyncSourceT<M, A>(items);
    
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    [Pure]
    public static SourceT<M, A> merge<M, A>(Seq<SourceT<M, A>> sources) 
        where M : MonadIO<M>, Fallible<M> =>
        sources.Fold((s, s2) => s.Choose(s2), empty<M, A>());
        
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    [Pure]
    public static SourceT<M, A> merge<M, A>(params SourceT<M, A>[] sources) 
        where M : MonadIO<M>, Fallible<M> =>
        merge(toSeq(sources));

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    [Pure]
    public static SourceT<M, (A First, B Second)> zip<M, A, B>(SourceT<M, A> first, SourceT<M, B> second) 
        where M : MonadUnliftIO<M>, Fallible<Error, M> =>
        new Zip2SourceT<M, A, B>(first, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    [Pure]
    public static SourceT<M, (A First, B Second, C Third)> zip<M, A, B, C>(SourceT<M, A> first, SourceT<M, B> second, SourceT<M, C> third) 
        where M : MonadUnliftIO<M>, Fallible<Error, M> =>
        new Zip3SourceT<M, A, B, C>(first, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    [Pure]
    public static SourceT<M, (A First, B Second, C Third, D Fourth)> zip<M, A, B, C, D>(SourceT<M, A> first, SourceT<M, B> second, SourceT<M, C> third, SourceT<M, D> fourth) 
        where M : MonadUnliftIO<M>, Fallible<Error, M> =>
        new Zip4SourceT<M, A, B, C, D>(first, second, third, fourth);

    /// <summary>
    /// Reduce the stream into a unit value.
    /// </summary>
    [Pure]
    public static K<M, Unit> iter<M, A>(K<SourceT<M>, A> ma) 
        where M : MonadIO<M> =>
        ma.Iter();

    /// <summary>
    /// Reduce the stream by collecting all the values into a `Seq` while the predicate holds.
    /// </summary>
    [Pure]
    public static K<M, Seq<A>> collectWhile<M, A>(K<SourceT<M>, A> ma, Func<(Seq<A> Items, A Item), bool> predicate) 
        where M : MonadIO<M> =>
        ma.CollectWhile(predicate);

    /// <summary>
    /// Reduce the stream by collecting all the values into a `Seq` while the predicate holds.
    /// </summary>
    [Pure]
    public static K<M, Seq<A>> collectUntil<M, A>(K<SourceT<M>, A> ma, Func<(Seq<A> Items, A Item), bool> predicate) 
        where M : MonadIO<M> =>
        ma.CollectUntil(predicate);

    /// <summary>
    /// Reduce the stream by collecting all the values into a `Seq`.
    /// </summary>
    [Pure]
    public static K<M, Seq<A>> collect<M, A>(K<SourceT<M>, A> ma) 
        where M : MonadIO<M> =>
        ma.Collect();
    
    /// <summary>
    /// Reduce the stream, yielding the last structure processed, or the `None` if the stream is empty
    /// </summary>
    [Pure]
    public static K<M, Option<A>> lastOrNone<M, A>(K<SourceT<M>, A> ma) 
        where M : MonadIO<M> =>
        ma.LastOrNone();
    
    /// <summary>
    /// Reduce the stream, yielding the first structure processed, or the `None` if the stream is empty
    /// </summary>
    [Pure]
    public static K<M, Option<A>> firstOrNone<M, A>(K<SourceT<M>, A> ma) 
        where M : MonadIO<M> =>
        ma.FirstOrNone();
    
    /// <summary>
    /// Reduce the stream, yielding the last structure processed, or the default value if the stream is empty
    /// </summary>
    [Pure]
    public static K<M, A> last<M, A>(K<SourceT<M>, A> ma, A defaultValue) 
        where M : MonadIO<M> =>
        ma.Last(defaultValue);

    /// <summary>
    /// Reduce the stream, yielding the first structure processed, or the default value if the stream is empty
    /// </summary>
    [Pure]
    public static K<M, A> first<M, A>(K<SourceT<M>, A> ma, A defaultValue) 
        where M : MonadIO<M> =>
        ma.First(defaultValue);
    
    /// <summary>
    /// Reduce the stream, yielding the last structure processed, or `M.Empty` if the stream is empty
    /// </summary>
    [Pure]
    public static K<M, A> last<M, A>(K<SourceT<M>, A> ma) 
        where M : MonadIO<M>, Alternative<M> =>
        ma.Last();
    
    /// <summary>
    /// Reduce the stream, yielding the first structure processed, or `M.Empty` if the stream is empty
    /// </summary>
    [Pure]
    public static K<M, A> first<M, A>(K<SourceT<M>, A> ma)
        where M : MonadIO<M>, Alternative<M> =>
        ma.First();
}
