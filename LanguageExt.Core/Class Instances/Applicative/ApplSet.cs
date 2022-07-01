#nullable enable

using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplSet<A, B> : Applicative<Set<Func<A, B>>, Set<A>, Set<B>, A, B>
    {
        public static readonly ApplSet<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Set<B> Action(Set<A> fa, Set<B> fb) =>
            fb;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Set<B> Apply(Set<Func<A, B>> fab, Set<A> fa)
        {
            return new Set<B>(Go());
            
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
        public Set<B> Map(Set<A> ma, Func<A, B> f) =>
            default(FSet<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Set<A> Pure(A x) =>
            Prelude.Set(x);
    }
}
