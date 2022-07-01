#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplFin<A, B> : 
        BiFunctor<Fin<A>, Fin<B>, A, Error, B, Error>,
        Applicative<Fin<Func<A, B>>, Fin<A>, Fin<B>, A, B>
    {
        public static readonly ApplFin<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, Error> fb) =>
            default(FFin<A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<B> Map(Fin<A> ma, Func<A, B> f) =>
            default(FFin<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<B> Apply(Fin<Func<A, B>> fab, Fin<A> fa) =>
            (fab, fa) switch
            {
                ({IsSucc: true} f, {IsSucc: true} a) => f.Value(a.Value),
                ({IsSucc: true}, {IsSucc: false} a) => a.Error,
                ({IsSucc: false} f, {IsSucc: true}) => f.Error,
                var (f, a) => f.Error + a.Error
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<A> Pure(A x) =>
            MFin<A>.Inst.Return(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<B> Action(Fin<A> fa, Fin<B> fb) =>
            (fa, fb) switch
            {
                ({IsSucc: true}, {IsSucc: true} b) => b,
                ({IsSucc: true}, {IsSucc: false} b) => b.Error,
                ({IsSucc: false} a, {IsSucc: true}) => a.Error,
                var (a, b) => a.Error + b.Error
            };
    }
}
