
using System;
using LanguageExt.Common;

namespace LanguageExt
{
    public interface TryCase<A>
    {
    }

    public sealed class FailCase<A> : TryCase<A>
    {
        public readonly Error Error;

        internal FailCase(Error error) =>
            Error = error;

        internal static TryCase<A> New(Error error) =>
            new FailCase<A>(error);

        internal static TryCase<A> New(Exception exception) =>
            new FailCase<A>(Common.Error.New(exception));

        public void Deconstruct(out Error Error) =>
            Error = this.Error;
    }

    public sealed class SuccCase<A> : TryCase<A>
    {
        public readonly A Value;

        internal SuccCase(A value) =>
            Value = value;

        internal static TryCase<A> New(A value) =>
            new SuccCase<A>(value);

        public void Deconstruct(out A Value) =>
            Value = this.Value;
    }
}
