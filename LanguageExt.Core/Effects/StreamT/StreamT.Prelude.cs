/*
#pragma warning disable LX_StreamT

using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// StreamT module
/// </summary>
public static partial class Prelude
{
    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> merge<M, A>(K<StreamT<M>, A> first, K<StreamT<M>, A> second)
        where M : Monad<M> =>
        first.Merge(second);

    /// <summary>
    /// Interleave the items of many streams
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="rest">N streams to merge</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> merge<M, A>(K<StreamT<M>, A> first, K<StreamT<M>, A> second, params K<StreamT<M>, A>[] rest)
        where M : Monad<M> =>
        first.Merge(second, rest);
 
    /// <summary>
    /// Merge the items of two streams into pairs
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second)> zip<M, A, B>(K<StreamT<M>, A> first, K<StreamT<M>, B> second)
        where M : Monad<M> =>
        first.Zip(second);

    /// <summary>
    /// Merge the items of two streams into 3-tuples
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second, C Third)> zip<M, A, B, C>(
        K<StreamT<M>, A> first, 
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third) 
        where M : Monad<M> =>
        first.Zip(second, third);

    /// <summary>
    /// Merge the items of two streams into 4-tuples
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <param name="fourth">Fourth stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second, C Third, D Fourth)> zip<M, A, B, C, D>(
        K<StreamT<M>, A> first,
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third,
        K<StreamT<M>, D> fourth)
        where M : Monad<M> =>
        first.Zip(second, third, fourth);
}
*/
