using System.Collections.Generic;
using System.Threading.Channels;
using static LanguageExt.Prelude;
namespace LanguageExt;

public partial class Source
{
    /// <summary>
    /// Empty source
    /// </summary>
    /// <remarks>
    /// This is a 'void' source, it yields zero values. 
    /// </remarks>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Uninhabited source</returns>
    public static Source<A> empty<A>() =>
        EmptySource<A>.Default;
    
    /// <summary>
    /// Lift a pure value into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one value. 
    /// </remarks>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    public static Source<A> pure<A>(A value) =>
        new PureSource<A>(value);
    
    /// <summary>
    /// Lift a pure value into the source and yield it for infinity
    /// </summary>
    /// <remarks>
    /// This is an infinite source, it repeatedly yields a value. 
    /// </remarks>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Infinite source</returns>
    public static Source<A> forever<A>(A value) =>
        new ForeverSource<A>(value);

    /// <summary>
    /// Make a `System.Threading.Channels.Channel` into a source of values
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static Source<A> lift<A>(Channel<A> channel) =>
        new ReaderSource<A>(channel);

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static Source<A> lift<A>(IEnumerable<A> items) =>
        new IteratorSyncSource<A>(items);

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static Source<A> lift<A>(IAsyncEnumerable<A> items) =>
        new IteratorAsyncSource<A>(items);
    
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    public static Source<A> merge<A>(Seq<Source<A>> sources) =>
        sources.Fold(empty<A>(), (s, s2) => s.Choose(s2));
        
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    public static Source<A> merge<A>(params Source<A>[] sources) =>
        merge(toSeq(sources));

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second)> zip<A, B>(Source<A> first, Source<B> second) =>
        new Zip2Source<A, B>(first, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third)> zip<A, B, C>(Source<A> first, Source<B> second, Source<C> third) =>
        new Zip3Source<A, B, C>(first, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third, D Fourth)> zip<A, B, C, D>(Source<A> first, Source<B> second, Source<C> third, Source<D> fourth) =>
        new Zip4Source<A, B, C, D>(first, second, third, fourth);
}
