#nullable enable
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class ReaderT
    {
        public static Reader<Env, Seq<B>> Traverse<Env, A, B>(this Seq<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Prelude.toSeq);

        public static Reader<Env, Lst<B>> Traverse<Env, A, B>(this Lst<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toList);

        public static Reader<Env, Arr<B>> Traverse<Env, A, B>(this Arr<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toArray);

        public static Reader<Env, B[]> Traverse<Env, A, B>(this Reader<Env, A>[] ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(x => x.ToArray());

        public static Reader<Env, Set<B>> Traverse<Env, A, B>(this Set<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toSet);

        public static Reader<Env, HashSet<B>> Traverse<Env, A, B>(this HashSet<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toHashSet);

        public static Reader<Env, Stck<B>> Traverse<Env, A, B>(this Stck<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma.Reverse(), f).Map(toStack);

        public static Reader<Env, IEnumerable<B>> Traverse<Env, A, B>(this IEnumerable<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);        

        internal static Reader<Env, List<B>> TraverseFast<Env, A, B>(this IEnumerable<Reader<Env, A>> ma, Func<A, B> f) => env =>
        {
            var values = new List<B>();
            foreach (var item in ma)
            {
                var resA = item(env);
                if (resA.IsFaulted) return ReaderResult<List<B>>.New(resA.ErrorInt);
                values.Add(f(resA.Value));
            }
            return ReaderResult<List<B>>.New(values);
        };        
    }
}
