using System;
using LanguageExt;

public static class __TupleExt
{
    public static R With<T1, T2, R>(this Tuple<T1, T2> self, Func<T1, T2, R> func)
    {
        return func(self.Item1, self.Item2);
    }

    public static R With<T1, T2, T3, R>(this Tuple<T1, T2, T3> self, Func<T1, T2, T3, R> func)
    {
        return func(self.Item1, self.Item2, self.Item3);
    }

    public static R With<T1, T2, T3, T4, R>(this Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func)
    {
        return func(self.Item1, self.Item2, self.Item3, self.Item4);
    }

    public static R With<T1, T2, T3, T4, T5, R>(this Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func)
    {
        return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
    }

    public static R With<T1, T2, T3, T4, T5, T6, R>(this Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func)
    {
        return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
    }

    public static R With<T1, T2, T3, T4, T5, T6, T7, R>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func)
    {
        return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
    }

    public static Unit With<T1, T2>(this Tuple<T1, T2> self, Action<T1, T2> func)
    {
        func(self.Item1, self.Item2);
        return Unit.Default;
    }

    public static Unit With<T1, T2, T3>(this Tuple<T1, T2, T3> self, Action<T1, T2, T3> func)
    {
        func(self.Item1, self.Item2, self.Item3);
        return Unit.Default;
    }

    public static Unit With<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4);
        return Unit.Default;
    }

    public static Unit With<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
        return Unit.Default;
    }

    public static Unit With<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        return Unit.Default;
    }

    public static Unit With<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        return Unit.Default;
    }
}