using System;
using System.Collections.Generic;

namespace LanguageExt.Pipes
{
    internal enum EnumerateDataType
    {
        Enumerable,
        AsyncEnumerable,
        Observable
    }

    internal abstract class EnumerateData<A>
    {
        public EnumerateDataType Type;

        protected EnumerateData(EnumerateDataType type) =>
            Type = type;
    }

    internal sealed class EnumerateEnumerable<A> : EnumerateData<A>
    {
        public readonly IEnumerable<A> Values;
        public EnumerateEnumerable(IEnumerable<A> values) : base(EnumerateDataType.Enumerable) =>
            Values = values;
    }

    internal sealed class EnumerateAsyncEnumerable<A> : EnumerateData<A>
    {
        public readonly IAsyncEnumerable<A> Values;
        public EnumerateAsyncEnumerable(IAsyncEnumerable<A> values) : base(EnumerateDataType.AsyncEnumerable) =>
            Values = values;
    }

    internal sealed class EnumerateObservable<A> : EnumerateData<A>
    {
        public readonly IObservable<A> Values;
        public EnumerateObservable(IObservable<A> values) : base(EnumerateDataType.Observable) =>
            Values = values;
    }
}
