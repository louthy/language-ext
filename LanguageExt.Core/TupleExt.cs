using System;
using LanguageExt;
using System.ComponentModel;
using System.Diagnostics.Contracts;

public static class __TupleExt
{
    /// <summary>
    /// Tuple map
    /// </summary>
    [Pure]
    public static R Map<T1, T2, T3, T4, R>(this Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func) =>
        func(self.Item1, self.Item2, self.Item3, self.Item4);

    /// <summary>
    /// Tuple map
    /// </summary>
    [Pure]
    public static R Map<T1, T2, T3, T4, T5, R>(this Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func) =>
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

    /// <summary>
    /// Tuple map
    /// </summary>
    [Pure]
    public static R Map<T1, T2, T3, T4, T5, T6, R>(this Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func) =>
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Tuple map
    /// </summary>
    [Pure]
    public static R Map<T1, T2, T3, T4, T5, T6, T7, R>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Tuple iterate
    /// </summary>
    public static Unit Iter<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4);
        return Unit.Default;
    }

    /// <summary>
    /// Tuple iterate
    /// </summary>
    public static Unit Iter<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
        return Unit.Default;
    }

    /// <summary>
    /// Tuple iterate
    /// </summary>
    public static Unit Iter<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        return Unit.Default;
    }

    /// <summary>
    /// Tuple iterate
    /// </summary>
    public static Unit Iter<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        return Unit.Default;
    }
}