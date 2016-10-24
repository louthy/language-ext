using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple VTuple() =>
            ValueTuple.Create();

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1> VTuple<T1>(T1 item1) =>
            ValueTuple.Create(item1);

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2> VTuple<T1, T2>(T1 item1, T2 item2) =>
            ValueTuple.Create(item1, item2);

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, T3> VTuple<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            ValueTuple.Create(item1, item2, item3);

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4> VTuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            ValueTuple.Create(item1, item2, item3, item4);

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5> VTuple<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) =>
            ValueTuple.Create(item1, item2, item3, item4, item5);

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5, T6> VTuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) =>
            ValueTuple.Create(item1, item2, item3, item4, item5, item6);

        /// <summary>
        /// Tuple constructor
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, T3, T4, T5, T6, T7> VTuple<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) =>
            ValueTuple.Create(item1, item2, item3, item4, item5, item6, item7);


        /// <summary>
        /// Tuple map
        /// </summary>
        [Pure]
        public static R map<T1, T2, T3, T4, R>(ValueTuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4);

        /// <summary>
        /// Tuple map
        /// </summary>
        [Pure]
        public static R map<T1, T2, T3, T4, T5, R>(ValueTuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

        /// <summary>
        /// Tuple map
        /// </summary>
        [Pure]
        public static R map<T1, T2, T3, T4, T5, T6, R>(ValueTuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

        /// <summary>
        /// Tuple map
        /// </summary>
        [Pure]
        public static R map<T1, T2, T3, T4, T5, T6, T7, R>(ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Tuple iterate
        /// </summary>
        public static Unit iter<T1, T2, T3, T4>(ValueTuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4);
            return Unit.Default;
        }

        /// <summary>
        /// Tuple iterate
        /// </summary>
        public static Unit iter<T1, T2, T3, T4, T5>(ValueTuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
            return Unit.Default;
        }

        /// <summary>
        /// Tuple iterate
        /// </summary>
        public static Unit iter<T1, T2, T3, T4, T5, T6>(ValueTuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
            return Unit.Default;
        }

        /// <summary>
        /// Tuple iterate
        /// </summary>
        public static Unit iter<T1, T2, T3, T4, T5, T6, T7>(ValueTuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
            return Unit.Default;
        }
    }
}
