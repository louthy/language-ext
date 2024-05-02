﻿using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Arr
    {
        /// <summary>
        /// Create an empty array
        /// </summary>
        [Pure]
        public static Arr<T> empty<T>() =>
            Arr<T>.Empty;

        /// <summary>
        /// Create a new empty array
        /// </summary>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> create<T>() =>
            Arr<T>.Empty;

        /// <summary>
        /// Create an array from a initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> create<T>(params T[] items) =>
            new Arr<T>(items);
        
        /// <summary>
        /// Create an array from a initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> create<T>(
            ReadOnlySpan<T> items) =>
            create(
                items.ToArray());

        /// <summary>
        /// Create an array from an initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> createRange<T>(IEnumerable<T> items) =>
            new Arr<T>(items);

        /// <summary>
        /// Add an item to the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="value">Item to add</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> add<T>(Arr<T> array, T value) =>
            array.Add(value);

        /// <summary>
        /// Add a range of items to the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="value">Items to add</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> addRange<T>(Arr<T> array, IEnumerable<T> value) =>
            array.AddRange(value);

        /// <summary>
        /// Remove an item from the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="value">value to remove</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> remove<T>(Arr<T> array, T value) =>
            array.Remove(value);

        /// <summary>
        /// Remove an item at a specified index in the array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="index">Index of item to remove</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> removeAt<T>(Arr<T> array, int index) =>
            array.RemoveAt(index);

        /// <summary>
        /// Reverses the array (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">Array item type</typeparam>
        /// <param name="array">Array to reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static T[] rev<T>(T[] array) =>
            array.Reverse().ToArray();

        /// <summary>
        /// Reverses the array (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">Array item type</typeparam>
        /// <param name="array">Array to reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static Arr<T> rev<T>(Arr<T> array) =>
            array.Reverse();

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static A[] flatten<A>(A[][] ma) =>
            ma.Bind(identity).ToArray();

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static Arr<A> flatten<A>(Arr<Arr<A>> ma) =>
            ma.Bind(identity);
    }
}
