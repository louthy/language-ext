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
    public readonly struct ApplEnumerable<A, B> : Applicative<IEnumerable<Func<A, B>>, IEnumerable<A>, IEnumerable<B>, A, B>
    {
        public static readonly ApplEnumerable<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<B> Action(IEnumerable<A> fa, IEnumerable<B> fb)
        {
            foreach (var x in fa)
            {
                // ignore
            }
            return fb;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<B> Apply(IEnumerable<Func<A, B>> fab, IEnumerable<A> fa)
        {
            foreach (var f in fab)
            {
                foreach (var a in fa)
                {
                    yield return f(a);
                }
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<B> Map(IEnumerable<A> ma, Func<A, B> f) =>
            default(FEnumerable<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> Pure(A x) =>
            new[] {x};
    }
}
