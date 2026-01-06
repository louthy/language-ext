#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using LanguageExt.Traits;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public static partial class SeqExtensions
{
    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(K<Seq, A> list)
    {
        [Obsolete("Use Reverse() instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [OverloadResolutionPriority(Change.Priority)]
        public Seq<A> Rev() =>
            Seq.rev(+list);

        [Obsolete("Use Combine() or the `+` operator instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [OverloadResolutionPriority(Change.Priority)]
        public Seq<A> Append(Seq<A> rhs) =>
            Seq.append(+list, rhs);

        [Obsolete("Use Combine() or the `+` operator instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [OverloadResolutionPriority(Change.Priority)]
        public Seq<A> Append(Seq<Seq<A>> xs) =>
            Seq.append(+list, xs);
    }
}
