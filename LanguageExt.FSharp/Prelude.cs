using System;
using System.Collections.Generic;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using static LanguageExt.Prelude;
using System.Collections.Immutable;

namespace LanguageExt
{
    public static class FSharp
    {
        /// <summary>
        /// Convert a LanguageExt Option into a F# Option
        /// </summary>
        public static Option<T> fs<T>(FSharpOption<T> fsOption) =>
            FSharpOption<T>.get_IsSome(fsOption)
                ? Some(fsOption.Value)
                : None;

        /// <summary>
        /// Convert an F# Option into a LanguageExt Option
        /// </summary>
        public static FSharpOption<T> fs<T>(this Option<T> option) =>
            option.IsNone
                ? FSharpOption<T>.None
                : match(option,
                     Some: v =>  FSharpOption<T>.Some(v),
                     None: () => failwith<FSharpOption<T>>("returns null, so can't use the None branch"));

        /// <summary>
        /// Convert an F# Option into a LanguageExt Option
        /// </summary>
        public static FSharpOption<T> fs<T>(this OptionUnsafe<T> option) =>
            matchUnsafe(option,
                Some: v => FSharpOption<T>.Some(v),
                None: () => FSharpOption<T>.None);

        /// <summary>
        /// Convert an F# List into an IEnumerable<T>
        /// </summary>
        public static IEnumerable<T> fs<T>(FSharpList<T> fsList) =>
            ListModule.ToSeq(fsList);

        /// <summary>
        /// Convert an LanguageExt List (IImmutableList<T>) into an F# List
        /// </summary>
        public static FSharpList<T> fs<T>(IImmutableList<T> list) =>
            ListModule.OfSeq(list);

        /// <summary>
        /// Convert an F# Map into a LanguageExt Map
        /// </summary>
        public static IImmutableDictionary<K, V> fs<K, V>(FSharpMap<K, V> fsMap) =>
            Map.addRange( map<K, V>(), List.map(fsMap, identity) );

        /// <summary>
        /// Convert a LanguageExt Map into an F# Map
        /// </summary>
        public static FSharpMap<K, V> fs<K, V>(IImmutableDictionary<K, V> map) =>
            MapModule.OfSeq(List.map(map, kv => Tuple.Create(kv.Key, kv.Value)));
    }
}
