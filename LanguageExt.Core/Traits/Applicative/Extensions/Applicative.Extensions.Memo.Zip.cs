using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    extension<F, A, B>((Memo<F, A> First, Memo<F, B> Second) tuple) where F : Applicative<F>
    {
        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second)> Zip() =>
            map((A a, B b) => (a, b), tuple.First).Apply(tuple.Second);
    }

    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    extension<F, A, B, C>((Memo<F, A> First, Memo<F, B> Second, Memo<F, C> Third) tuple) where F : Applicative<F>
    {
        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second, C Third)> Zip() =>
            map((A a, B b, C c) => (a, b, c), tuple.First)
               .Apply(tuple.Second)
               .Apply(tuple.Third);
    }

    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    extension<F, A, B, C, D>((Memo<F, A> First, Memo<F, B> Second, Memo<F, C> Third, Memo<F, D> Fourth) tuple) where F : Applicative<F>
    {
        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second, C Third, D Fourth)> Zip() =>
            map((A a, B b, C c, D d) => (a, b, c, d), tuple.First)
               .Apply(tuple.Second)
               .Apply(tuple.Third)
               .Apply(tuple.Fourth);
    }

    /// <param name="tuple">Tuple of applicatives to run</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    /// <typeparam name="B">Second applicative's bound value type</typeparam>
    /// <typeparam name="C">Third applicative's bound value type</typeparam>
    /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
    extension<F, A, B, C, D, E>((Memo<F, A> First, Memo<F, B> Second, Memo<F, C> Third, Memo<F, D> Fourth, Memo<F, E> Fifth) tuple) where F : Applicative<F>
    {
        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second, C Third, D Fourth, E Fifth)> Zip() =>
            map((A a, B b, C c, D d, E e) => (a, b, c, d, e), tuple.First)
               .Apply(tuple.Second)
               .Apply(tuple.Third)
               .Apply(tuple.Fourth)
               .Apply(tuple.Fifth);
    }


    /// <param name="First">First applicative</param>
    /// <typeparam name="F">Applicative trait type</typeparam>
    /// <typeparam name="A">First applicative's bound value type</typeparam>
    extension<F, A>(Memo<F, A> First) where F : Applicative<F>
    {
        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <param name="Second">Second applicative</param>
        /// <typeparam name="B">Second applicative's bound value type</typeparam>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second)> Zip<B>(Memo<F, B> Second) =>
            map((A a, B b) => (a, b), First).Apply(Second);

        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <param name="Second">Second applicative</param>
        /// <param name="Third">Third applicative</param>
        /// <typeparam name="B">Second applicative's bound value type</typeparam>
        /// <typeparam name="C">Third applicative's bound value type</typeparam>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second, C Third)> Zip<B, C>(Memo<F, B> Second, Memo<F, C> Third) =>
            map((A a, B b, C c) => (a, b, c), First)
               .Apply(Second)
               .Apply(Third);

        /// <summary>
        /// Zips applicatives into a tuple
        /// </summary>
        /// <param name="Second">Second applicative</param>
        /// <param name="Third">Third applicative</param>
        /// <param name="Fourth">Fourth applicative</param>
        /// <typeparam name="B">Second applicative's bound value type</typeparam>
        /// <typeparam name="C">Third applicative's bound value type</typeparam>
        /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second, C Third, D Fourth)> Zip<B, C, D>(Memo<F, B> Second, Memo<F, C> Third, Memo<F, D> Fourth) =>
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
        /// <typeparam name="B">Second applicative's bound value type</typeparam>
        /// <typeparam name="C">Third applicative's bound value type</typeparam>
        /// <typeparam name="D">Fourth applicative's bound value type</typeparam>
        /// <returns>Zipped applicative</returns>
        public K<F, (A First, B Second, C Third, D Fourth, E Fifth)> Zip<B, C, D, E>(Memo<F, B> Second, Memo<F, C> Third, Memo<F, D> Fourth, Memo<F, E> Fifth) =>
            map((A a, B b, C c, D d, E e) => (a, b, c, d, e), First)
               .Apply(Second)
               .Apply(Third)
               .Apply(Fourth)
               .Apply(Fifth);
    }
}
