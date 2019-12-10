using System.Collections.Immutable;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Benchmarks
{
    internal partial class ValuesGenerator
    {
        public static ImmutableHashSet<T> SysColImmutableHashSetSetup<T>(T[] values)
        {
            var immutableSet = ImmutableHashSet.Create<T>();
            foreach (var value in values)
            {
                immutableSet = immutableSet.Add(value);
            }

            return immutableSet;
        }

        public static ImmutableSortedSet<T> SysColImmutableSortedSetSetup<T>(T[] values)
        {
            var immutableSet = ImmutableSortedSet.Create<T>();
            foreach (var value in values)
            {
                immutableSet = immutableSet.Add(value);
            }

            return immutableSet;
        }

        public static System.Collections.Generic.HashSet<T> SysColHashSetSetup<T>(T[] values)
        {
            var hashSet = new System.Collections.Generic.HashSet<T>();
            foreach (var value in values)
            {
                hashSet.Add(value);
            }

            return hashSet;
        }

        public static HashSet<TEq, T> LangExtHashSetSetup<T, TEq>(T[] values)
            where TEq : struct, Eq<T>
        {
            var hashSet = HashSet<TEq, T>();
            foreach (var value in values)
            {
                hashSet = hashSet.Add(value);
            }

            return hashSet;
        }

        public static Set<TOrd, T> LangExtSetSetup<T, TOrd>(T[] values)
            where TOrd : struct, Ord<T>
        {
            var set = Set<TOrd, T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }
    }
}
