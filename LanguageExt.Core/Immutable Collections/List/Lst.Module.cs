#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public partial class Lst
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Lst<A> flatten<A>(Lst<Lst<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Create an empty Lst T
    /// </summary>
    [Pure]
    public static Lst<T> empty<T>() =>
        Lst<T>.Empty;

    /// <summary>
    /// Create a singleton collection
    /// </summary>
    /// <param name="value">Single value</param>
    /// <returns>Collection with a single item in it</returns>
    [Pure]
    public static Lst<A> singleton<A>(A value) =>
        [value];

    /// <summary>
    /// Create a new empty list
    /// </summary>
    /// <returns>Lst T</returns>
    [Pure]
    public static Lst<T> create<T>() =>
        Lst<T>.Empty;

    /// <summary>
    /// Create a list from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>Lst T</returns>
    [Pure]
    public static Lst<T> create<T>(params T[] items) =>
        new (items.AsSpan());

    /// <summary>
    /// Create a list from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>Lst T</returns>
    [Pure]
    public static Lst<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.IsEmpty
            ? Lst<A>.Empty
            : new (items);

    /// <summary>
    /// Create a list from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>Lst T</returns>
    [Pure]
    public static Lst<A> createRange<A>(IEnumerable<A> items) =>
        new (items);

    /// <summary>
    /// Add an item to the list
    /// </summary>
    /// <param name="list">List</param>
    /// <param name="value">Item to add</param>
    /// <returns>A new Lst T</returns>
    [Pure]
    public static Lst<T> add<T>(Lst<T> list, T value) =>
        list.Add(value);

    /// <summary>
    /// Remove an item from the list
    /// </summary>
    /// <param name="list">List</param>
    /// <param name="value">value to remove</param>
    /// <returns>A new Lst T</returns>
    [Pure]
    public static Lst<T> remove<T>(Lst<T> list, T value) =>
        list.Remove(value);

    /// <summary>
    /// Remove an item at a specified index in the list
    /// </summary>
    /// <param name="list">List</param>
    /// <param name="index">Index of item to remove</param>
    /// <returns>A new Lst T</returns>
    [Pure]
    public static Lst<T> removeAt<T>(Lst<T> list, int index) =>
        list.RemoveAt(index);

    /// <summary>
    /// Reverses the list (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="list">List to reverse</param>
    /// <returns>Reversed list</returns>
    [Pure]
    public static Lst<T> rev<T>(Lst<T> list) =>
        list.Reverse();
}
