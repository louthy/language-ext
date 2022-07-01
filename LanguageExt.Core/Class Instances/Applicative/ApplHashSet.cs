#nullable enable

using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplHashSet<A, B> :
        Applicative<HashSet<Func<A, B>>, HashSet<A>, HashSet<B>, A, B>
    {
        public static readonly ApplHashSet<A, B> Inst = default(ApplHashSet<A, B>);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashSet<B> Action(HashSet<A> fa, HashSet<B> fb) =>
            fb;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashSet<B> Apply(HashSet<Func<A, B>> fab, HashSet<A> fa)
        {
            return new HashSet<B>(Go());
            IEnumerable<B> Go()
            {
                foreach (var f in fab)
                {
                    foreach (var a in fa)
                    {
                        yield return f(a);
                    }
                }
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashSet<B> Map(HashSet<A> ma, Func<A, B> f) =>
            default(FHashSet<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashSet<A> Pure(A x) =>
            HashSet(x);
    }
}
