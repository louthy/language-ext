using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public static partial class Prelude
{
    [Pure]
    public static NEWTYPE bind<NEWTYPE, T, PRED>(NewType<NEWTYPE, T, PRED> value, Func<T, NEWTYPE> bind)
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
        value.Bind(bind);

    public static Unit iter<NEWTYPE, T, PRED>(NewType<NEWTYPE, T, PRED> value, Action<T> f)
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
        value.Iter(f);

    [Pure]
    public static bool exists<NEWTYPE, T, PRED>(NewType<NEWTYPE, T, PRED> value, Func<T, bool> predicate)
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
        predicate((T)value);

    [Pure]
    public static bool forall<NEWTYPE, T, PRED>(NewType<NEWTYPE, T, PRED> value, Func<T, bool> predicate)
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
        predicate((T)value);

    [Pure]
    public static NEWTYPE map<NEWTYPE, T, PRED>(NewType<NEWTYPE, T, PRED> value, Func<T, T> map)
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
        value.Map(map);

    [Pure]
    public static S fold<NEWTYPE, T, S, PRED>(NewType<NEWTYPE, T, PRED> value, S state, Func<S, T, S> folder)
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
        value.Fold(state, folder);
}
