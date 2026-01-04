using System;
using LanguageExt.Common;

namespace LanguageExt;

public static partial class Prelude
{
    [Obsolete("Use toLst instead")]
    public static Lst<T> toList<T>(Option<T> option) =>
        option.ToList();
    
    [Obsolete("FinSucc has been deprecated in favour of `Fin.Succ` or `Prelude.Pure`")]
    public static Fin<A> FinSucc<A>(A value) =>
        new Fin<A>.Succ(value);

    [Obsolete("FinFail has been deprecated in favour of `Fin.Fail` or `Prelude.Fail`")]
    public static Fin<A> FinFail<A>(Error value) =>
        new Fin<A>.Fail(value);
}
