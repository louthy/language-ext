using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LE = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    [Obsolete(Change.UseCollectionIntialiserSeq)]
    public static Seq<A> Seq1<A>(A value)
    {
        var arr = new A[4];
        arr[2] = value;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 1, 0, 0));
    }

    [Obsolete(Change.NullableMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<A> toSeq<A>(A? value) where A : struct =>
        value is null 
            ? Empty 
            : LE.Seq.FromSingleValue(value.Value);

    [Obsolete(Change.NullableMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<R> toSeq<L, R>(Either<L, R> value) =>
        value.IsRight
            ? LE.Seq.FromSingleValue(value.RightValue)
            : Empty;
    
}
