using static LanguageExt.Prelude;
namespace LanguageExt.Pipes.Concurrent;

public partial class Source
{
    /// <summary>
    /// Lift a pure value into the source
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    public static Source<A> pure<A>(A value) =>
        new SourcePure<A>(value);

    /// <summary>
    /// Empty source
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Uninhabited source</returns>
    public static Source<A> empty<A>() =>
        SourceEmpty<A>.Default;
    
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    public static Source<A> merge<A>(Seq<Source<A>> sources) =>
        sources.Fold(empty<A>(), (s, s2) => s.Combine(s2));
        
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
        new SourceZip2<A, B>(first, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third)> zip<A, B, C>(Source<A> first, Source<B> second, Source<C> third) =>
        new SourceZip3<A, B, C>(first, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third, D Fourth)> zip<A, B, C, D>(Source<A> first, Source<B> second, Source<C> third, Source<D> fourth) =>
        new SourceZip4<A, B, C, D>(first, second, third, fourth);
}
