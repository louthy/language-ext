using System;
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
        /// Create an empty IEnumerable T
        /// </summary>
        [Pure]
        public static Arr<T> empty<T>() =>
            Arr<T>.Empty;

        /// <summary>
        /// Create a new empty list
        /// </summary>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> create<T>() =>
            Arr<T>.Empty;

        /// <summary>
        /// Create a list from a initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> create<T>(params T[] items) =>
            new Arr<T>(items);

        /// <summary>
        /// Create a list from an initial set of items
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Lst T</returns>
        [Pure]
        public static Arr<T> createRange<T>(IEnumerable<T> items) =>
            new Arr<T>(items);

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">Item to add</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> add<T>(Arr<T> list, T value) =>
            list.Add(value);

        /// <summary>
        /// Add a range of items to the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">Items to add</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> addRange<T>(Arr<T> list, IEnumerable<T> value) =>
            list.AddRange(value);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">value to remove</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> remove<T>(Arr<T> list, T value) =>
            list.Remove(value);

        /// <summary>
        /// Remove an item at a specified index in the list
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="index">Index of item to remove</param>
        /// <returns>A new Lst T</returns>
        [Pure]
        public static Arr<T> removeAt<T>(Arr<T> list, int index) =>
            list.RemoveAt(index);

        /// <summary>
        /// Reverses the list (Reverse in LINQ)
        /// </summary>
        /// <typeparam name="T">List item type</typeparam>
        /// <param name="list">List to reverse</param>
        /// <returns>Reversed list</returns>
        [Pure]
        public static Arr<T> rev<T>(Arr<T> list) =>
            list.Reverse();
    }
}