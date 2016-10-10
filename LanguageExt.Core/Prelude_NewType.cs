using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static NewType<T> bind<T>(NewType<T> value, Func<T, NewType<T>> bind) =>
            value.Bind(bind);

        public static Unit iter<T>(NewType<T> value, Action<T> f) =>
            value.Iter(f);

        [Pure]
        public static int count<T>(NewType<T> value) =>
            1;

        [Pure]
        public static int sum(NewType<int> value) =>
            value.Value;

        [Pure]
        public static bool exists<T>(NewType<T> value, Func<T, bool> predicate) =>
            predicate(value.Value);

        [Pure]
        public static bool forall<T>(NewType<T> value, Func<T, bool> predicate) =>
            predicate(value.Value);

        [Pure]
        public static NewType<T> map<T>(NewType<T> value, Func<T, T> map) =>
            value.Map(map);

        [Pure]
        public static S fold<T, S>(NewType<T> value, S state, Func<S, T, S> folder) =>
            value.Fold(state, folder);

        [Pure]
        public static NewType<T> append<T>(NewType<T> value, NewType<T> rhs) =>
            value.Append(rhs);

        [Pure]
        public static NewType<T> subtract<T>(NewType<T> value, NewType<T> rhs) =>
            value.Subtract(rhs);

        [Pure]
        public static NewType<T> divide<T>(NewType<T> value, NewType<T> rhs) =>
            value.Divide(rhs);

        [Pure]
        public static NewType<T> multiply<T>(NewType<T> value, NewType<T> rhs) =>
            value.Multiply(rhs);
    }
}
