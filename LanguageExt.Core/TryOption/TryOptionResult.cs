using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    public static class TryOptionResult
    {
        [Pure]
        public static TryOptionResult<T> Cast<T>(T value) =>
            new TryOptionResult<T>(value);
    }

    public struct TryOptionResult<T>
    {
        internal readonly Option<T> Value;
        internal readonly Exception Exception;

        public TryOptionResult(Option<T> value)
        {
            Value = value;
            Exception = null;
        }

        public TryOptionResult(Exception e)
        {
            Exception = e;
            Value = default(Option<T>);
        }

        [Pure]
        public static implicit operator TryOptionResult<T>(Option<T> value) =>
            new TryOptionResult<T>(value);

        [Pure]
        public static implicit operator TryOptionResult<T>(T value) =>
            new TryOptionResult<T>(Optional(value));

        [Pure]
        public static implicit operator TryOptionResult<T>(OptionNone value) =>
            new TryOptionResult<T>(Option<T>.None);

        public static TryOptionResult<T> Fail = new TryOptionResult<T>(Option<T>.None);

        [Pure]
        internal bool IsFaulted => Exception != null;

        [Pure]
        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();
    }
}
