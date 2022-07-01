#nullable enable

using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplSeq<A, B> : Applicative<Seq<Func<A, B>>, Seq<A>, Seq<B>, A, B>
    {
        public static readonly ApplSeq<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<B> Action(Seq<A> fa, Seq<B> fb)
        {
            fa.Strict(); // Force eval
            return fb;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<B> Apply(Seq<Func<A, B>> fab, Seq<A> fa)
        {
            return new Seq<B>(Go());
            
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
        public Seq<B> Map(Seq<A> ma, Func<A, B> f) =>
            default(FSeq<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> Pure(A x) =>
            Seq.create(x);
    }
}
