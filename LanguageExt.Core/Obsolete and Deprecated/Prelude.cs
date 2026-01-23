using System;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt;

public static partial class Prelude
{
    [Obsolete("Use toLst instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Lst<T> toList<T>(Option<T> option) =>
        option.ToLst();
    
    [Obsolete("FinSucc has been deprecated in favour of `Fin.Succ` or `Prelude.Pure`")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Fin<A> FinSucc<A>(A value) =>
        new Fin<A>.Succ(value);

    [Obsolete("FinFail has been deprecated in favour of `Fin.Fail` or `Prelude.Fail`")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Fin<A> FinFail<A>(Error value) =>
        new Fin<A>.Fail(value);
}
