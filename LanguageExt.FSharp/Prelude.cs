using System;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class FSharp
{
    /// <summary>
    /// Convert an F# Option into a LanguageExt Option 
    /// </summary>
    public static Option<T> fs<T>(FSharpOption<T> fsOption) =>
        FSharpOption<T>.get_IsSome(fsOption)
            ? Some(fsOption.Value)
            : None;

    /// <summary>
    /// Convert a LanguageExt Option into an F# Option 
    /// </summary>
    public static FSharpOption<T> fs<T>(Option<T> option) =>
        option.Map(FSharpOption<T>.Some)
              .IfNone(FSharpOption<T>.None)!;

    /// <summary>
    /// Convert an F# List into an IEnumerable T
    /// </summary>
    public static Lst<T> fs<T>(FSharpList<T> fsList) =>
        List.createRange(fsList);

    /// <summary>
    /// Convert a LanguageExt List (Lst T) into an F# List
    /// </summary>
    public static FSharpList<T> fs<T>(Lst<T> list) =>
        ListModule.OfSeq(list);

    /// <summary>
    /// Convert an F# Map into a LanguageExt Map (Map K V)
    /// </summary>
    public static Map<K, V> fs<K, V>(FSharpMap<K, V> fsMap) =>
        Map.addRange( Map<K, V>(), List.map(fsMap, identity) );

    /// <summary>
    /// Convert a LanguageExt Map (Map K V) into an F# Map
    /// </summary>
    public static FSharpMap<K, V> fs<K, V>(Map<K, V> map) =>
        MapModule.OfSeq(map.AsIterable().Map(item => Tuple.Create(item.Key,item.Value)));

    /// <summary>
    /// Convert LanguageExt Unit to F# unit
    /// </summary>
    /// <param name="unit">()</param>
    public static void fs(Unit unit)
    {
    }

    /// <summary>
    /// Convert an F# Result into a LanguageExt Either
    /// </summary>
    public static Either<TError, T> fs<T, TError>(FSharpResult<T, TError> result) =>
        result.IsOk
            ? Either<TError, T>.Right(result.ResultValue)
            : Either<TError, T>.Left(result.ErrorValue);

    /// <summary>
    /// Convert a LanguageExt Either into an F# Result
    /// </summary>
    public static FSharpResult<R, L> fs<L, R>(Either<L, R> either) =>
        match(either,
              FSharpResult<R, L>.NewError,
              FSharpResult<R, L>.NewOk);

    /// <summary>
    /// Convert a LanguageExt Option into an F# Option 
    /// </summary>
    public static FSharpOption<T> ToFSharp<T>(this Option<T> option) =>
        option.IsNone
            ? FSharpOption<T>.None
            : match(option,
                    Some: FSharpOption<T>.Some,
                    None: () => failwith<FSharpOption<T>>("returns null, so can't use the None branch"));

    /// <summary>
    /// Convert a LanguageExt Map (Map K V) into an F# Map
    /// </summary>
    public static FSharpMap<K, V> ToFSharp<K, V>(this Map<K, V> map) =>
        MapModule.OfSeq(map.AsIterable().Map(item => Tuple.Create(item.Key, item.Value)));

    /// <summary>
    /// Convert a LanguageExt List (Lst A) into an F# List
    /// </summary>
    public static FSharpList<A> ToFSharp<A>(this Lst<A> list) =>
        ListModule.OfSeq(list);

    /// <summary>
    /// Convert a LanguageExt Either into an F# Result
    /// </summary>
    public static FSharpResult<R, L> ToFSharp<L, R>(this Either<L, R> either) =>
        match(either,
              FSharpResult<R, L>.NewError,
              FSharpResult<R, L>.NewOk);
}
