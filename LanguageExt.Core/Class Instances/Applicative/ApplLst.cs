#nullable enable

using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplLst<A, B> :
        Applicative<Lst<Func<A, B>>, Lst<A>, Lst<B>, A, B>
    {
        public static readonly ApplLst<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<B> Action(Lst<A> fa, Lst<B> fb) =>
            fb;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<B> Apply(Lst<Func<A, B>> fab, Lst<A> fa)
        {
            return new Lst<B>(Go());

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
        public Lst<B> Map(Lst<A> ma, Func<A, B> f) =>
            default(FLst<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Pure(A x) =>
            List.create(x);
    }
}
