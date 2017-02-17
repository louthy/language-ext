using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Monad value type-class
    /// </summary>
    [Typeclass]
    public interface MonadValue<MValue, State, A>
    {
        [Pure]
        (A, State, bool) Eval(MValue ma, State state);

        [Pure]
        MValue Lift((A, State, bool) value);

        [Pure]
        MValue Lift(Func<State, (A, State, bool)> f);

        [Pure]
        MValue Bottom { get; }
    }

    public interface ReaderMonadValue<MValue, R, A>
    {
        [Pure]
        (A, R, bool) Eval(MValue ma, R state);

        [Pure]
        MValue Lift((A, R, bool) value);

        [Pure]
        MValue Lift(Func<R, (A, R, bool)> f);

        [Pure]
        MValue Bottom { get; }
    }

    public interface WriterMonadValue<MValue, W, A>
    {
        [Pure]
        (A, W, bool) Eval(MValue ma, W state);

        [Pure]
        MValue Lift((A, W, bool) value);

        [Pure]
        MValue Lift(Func<W, (A, W, bool)> f);

        [Pure]
        MValue Bottom { get; }
    }

    public interface StateMonadValue<MValue, S, A>
    {
        [Pure]
        (A, S, bool) Eval(MValue ma, S state);

        [Pure]
        MValue Lift((A, S, bool) value);

        [Pure]
        MValue Lift(Func<S, (A, S, bool)> f);

        [Pure]
        MValue Bottom { get; }
    }
}
