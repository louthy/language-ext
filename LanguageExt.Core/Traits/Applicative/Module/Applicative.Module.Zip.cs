using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second)> zip<F, A, B>((K<F, A> First, K<F, B> Second) tuple) where F : Applicative<F> =>
        map((A a, B b) => (a, b), tuple.First).Apply(tuple.Second);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third)> zip<F, A, B, C>((K<F, A> First, K<F, B> Second, K<F, C> Third) tuple) where F : Applicative<F> =>
        map((A a, B b, C c) => (a, b, c), tuple.First)
           .Apply(tuple.Second)
           .Apply(tuple.Third);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth)> zip<F, A, B, C, D>((K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth) tuple) where F : Applicative<F> =>
        map((A a, B b, C c, D d) => (a, b, c, d), tuple.First)
           .Apply(tuple.Second)
           .Apply(tuple.Third)
           .Apply(tuple.Fourth);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth, E Fifth)> zip<F, A, B, C, D, E>((K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth, K<F, E> Fifth) tuple) where F : Applicative<F> =>
        map((A a, B b, C c, D d, E e) => (a, b, c, d, e), tuple.First)
           .Apply(tuple.Second)
           .Apply(tuple.Third)
           .Apply(tuple.Fourth)
           .Apply(tuple.Fifth);


    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="Second">Second applicative</param>
    /// <param name="First">First applicative</param>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second)> zip<F, A, B>(K<F, A> First, K<F, B> Second) where F : Applicative<F> =>
        map((A a, B b) => (a, b), First).Apply(Second);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="Second">Second applicative</param>
    /// <param name="Third">Third applicative</param>
    /// <param name="First">First applicative</param>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third)> zip<F, A, B, C>(K<F, A> First, K<F, B> Second, K<F, C> Third) where F : Applicative<F> =>
        map((A a, B b, C c) => (a, b, c), First)
           .Apply(Second)
           .Apply(Third);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="Second">Second applicative</param>
    /// <param name="Third">Third applicative</param>
    /// <param name="Fourth">Fourth applicative</param>
    /// <param name="First">First applicative</param>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth)> zip<F, A, B, C, D>(K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth) where F : Applicative<F> =>
        map((A a, B b, C c, D d) => (a, b, c, d), First)
           .Apply(Second)
           .Apply(Third)
           .Apply(Fourth);

    /// <summary>
    /// Zips applicatives into a tuple
    /// </summary>
    /// <param name="Second">Second applicative</param>
    /// <param name="Third">Third applicative</param>
    /// <param name="Fourth">Fourth applicative</param>
    /// <param name="Fifth">Fifth applicative</param>
    /// <param name="First">First applicative</param>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <returns>Zipped applicative</returns>
    public static K<F, (A First, B Second, C Third, D Fourth, E Fifth)> zip<F, A, B, C, D, E>(K<F, A> First, K<F, B> Second, K<F, C> Third, K<F, D> Fourth, K<F, E> Fifth) where F : Applicative<F> =>
        map((A a, B b, C c, D d, E e) => (a, b, c, d, e), First)
           .Apply(Second)
           .Apply(Third)
           .Apply(Fourth)
           .Apply(Fifth);
}
