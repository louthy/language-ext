using System.Collections.Immutable;
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

        public static HashSet<T> LangExtHashSetSetup<T>(T[] values)
        {
            var hashSet = HashSet<T>();
            foreach (var value in values)
            {
                hashSet = hashSet.Add(value);
            }

            return hashSet;
        }

        public static Set<T> LangExtSetSetup<T>(T[] values)
        {
            var set = Set<T>();
            foreach (var value in values)
            {
                set = set.Add(value);
            }

            return set;
        }
    }
}
