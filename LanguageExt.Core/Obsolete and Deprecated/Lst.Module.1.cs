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

public static partial class List
{
    [Obsolete("Please use Lst.flatten")]
    public static Lst<A> flatten<A>(Lst<Lst<A>> ma) =>
        ma.Bind(identity);

    [Obsolete("Please use Lst.empty")]
    public static Lst<T> empty<T>() =>
        Lst<T>.Empty;

    [Obsolete("Please use Lst.singleton")]
    public static Lst<A> singleton<A>(A value) =>
        [value];

    [Obsolete("Please use Lst.create")]
    public static Lst<T> create<T>() =>
        Lst<T>.Empty;

    [Obsolete("Please use Lst.create")]
    public static Lst<T> create<T>(params T[] items) =>
        new (items.AsSpan());

    [Obsolete("Please use Lst.createRange")]
    public static Lst<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.IsEmpty
            ? Lst<A>.Empty
            : new (items);

    [Obsolete("Please use Lst.add")]
    public static Lst<T> add<T>(Lst<T> list, T value) =>
        list.Add(value);

    [Obsolete("Please use Lst.remove")]
    public static Lst<T> remove<T>(Lst<T> list, T value) =>
        list.Remove(value);

    [Obsolete("Please use Lst.removeAt")]
    public static Lst<T> removeAt<T>(Lst<T> list, int index) =>
        list.RemoveAt(index);

    [Obsolete("Please use Lst.rev")]
    public static Lst<T> rev<T>(Lst<T> list) =>
        list.Reverse();
}
