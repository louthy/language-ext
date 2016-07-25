using System;
using System.Collections.Generic;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class FSharp
    {
        /// <summary>
        /// Convert a F# Option into a LanguageExt Option 
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
                  .IfNoneUnsafe(FSharpOption<T>.None);

        /// <summary>
        /// Convert a LanguageExt OptionUnsafe into an F# Option 
        /// </summary>
        public static FSharpOption<T> fs<T>(OptionUnsafe<T> option) =>
            option.Map(FSharpOption<T>.Some)
                  .IfNoneUnsafe(FSharpOption<T>.None);

        /// <summary>
        /// Convert an F# List into an IEnumerable T
        /// </summary>
        public static Lst<T> fs<T>(FSharpList<T> fsList) =>
            List.createRange(fsList);

        /// <summary>
        /// Convert an LanguageExt List (Lst T) into an F# List
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
            MapModule.OfSeq(map.AsEnumerable().Map(item => Tuple(item.Key,item.Value)));

        /// <summary>
        /// Convert LanguageExt Unit to F# unit
        /// </summary>
        /// <param name="unit">()</param>
        public static void fs(Unit unit)
        {
        }


        /// <summary>
        /// Convert a LanguageExt Option into an F# Option 
        /// </summary>
        public static FSharpOption<T> ToFSharp<T>(this Option<T> option) =>
            option.IsNone
                ? FSharpOption<T>.None
                : match(option,
                     Some: v => FSharpOption<T>.Some(v),
                     None: () => failwith<FSharpOption<T>>("returns null, so can't use the None branch"));

        /// <summary>
        /// Convert a LanguageExt OptionUnsafe into an F# Option 
        /// </summary>
        public static FSharpOption<T> ToFSharp<T>(this OptionUnsafe<T> option) =>
            matchUnsafe(option,
                Some: v => FSharpOption<T>.Some(v),
                None: () => FSharpOption<T>.None);

        /// <summary>
        /// Convert a LanguageExt Map (Map K V) into an F# Map
        /// </summary>
        public static FSharpMap<K, V> ToFSharp<K, V>(this Map<K, V> map) =>
            MapModule.OfSeq(map.AsEnumerable().Map(item => Tuple(item.Key, item.Value)));

        /// <summary>
        /// Convert an LanguageExt List (Lst A) into an F# List
        /// </summary>
        public static FSharpList<A> ToFSharp<A>(this Lst<A> list) =>
            ListModule.OfSeq(list);
    }
}
