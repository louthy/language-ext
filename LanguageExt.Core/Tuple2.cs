using System;
using LanguageExt;
using static LanguageExt.Prelude;

public static class __Tuple2
{
    /// <summary>
    /// Sum
    /// </summary>
    public static int Sum<T1, T2>(this Tuple<T1, T2> self) =>
        0;

    /// <summary>
    /// Sum
    /// </summary>
    public static int Sum<T1, T2>(this Tuple<int, T2> self) =>
        self.Item1;

    /// <summary>
    /// Sum
    /// </summary>
    public static int Sum<T1, T2>(this Tuple<T1, int> self) =>
        self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static int Sum<T1, T2>(this Tuple<int, int> self) =>
        self.Item1 + self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static double Sum<T1, T2>(this Tuple<double, T2> self) =>
        self.Item1;

    /// <summary>
    /// Sum
    /// </summary>
    public static double Sum<T1, T2>(this Tuple<T1, double> self) =>
        self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static double Sum<T1, T2>(this Tuple<double, double> self) =>
        self.Item1 + self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static float Sum<T1, T2>(this Tuple<float, T2> self) =>
        self.Item1;

    /// <summary>
    /// Sum
    /// </summary>
    public static float Sum<T1, T2>(this Tuple<T1, float> self) =>
        self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static float Sum<T1, T2>(this Tuple<float, float> self) =>
        self.Item1 + self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static decimal Sum<T1, T2>(this Tuple<decimal, T2> self) =>
        self.Item1;

    /// <summary>
    /// Sum
    /// </summary>
    public static decimal Sum<T1, T2>(this Tuple<T1, decimal> self) =>
        self.Item2;

    /// <summary>
    /// Sum
    /// </summary>
    public static decimal Sum<T1, T2>(this Tuple<decimal, decimal> self) =>
        self.Item1 + self.Item2;

    /// <summary>
    /// Map tuple to R
    /// </summary>
    public static R Map<T1, T2, R>(this Tuple<T1, T2> self, Func<T1, T2, R> map) =>
        map(self.Item1, self.Item2);

    /// <summary>
    /// Map tuple to tuple
    /// </summary>
    public static Tuple<R1, R2> Map<T1, T2, R1, R2>(this Tuple<T1, T2> self, Func<Tuple<T1, T2>, Tuple<R1, R2>> map) =>
        map(self);

    /// <summary>
    /// Map tuple to tuple
    /// </summary>
    public static Tuple<R1, R2> Select<T1, T2, R1, R2>(this Tuple<T1, T2> self, Func<Tuple<T1, T2>, Tuple<R1, R2>> map) =>
        map(self);

    /// <summary>
    /// Tuple iterate
    /// </summary>
    public static Unit Iter<T1, T2>(this Tuple<T1, T2> self, Action<T1, T2> func)
    {
        func(self.Item1, self.Item2);
        return Unit.Default;
    }
}