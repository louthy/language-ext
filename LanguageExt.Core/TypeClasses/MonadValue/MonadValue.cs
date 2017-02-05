using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Monad value type-class
    /// </summary>
    [Typeclass]
    public interface MonadValue<MValue, State, A>
    {
        (A, State, bool) Eval(MValue ma, State state);
        MValue Lift((A, State, bool) value);
        MValue Lift(Func<State, (A, State, bool)> f);
        MValue Bottom { get; }
    }

    public interface ReaderMonadValue<MValue, R, A>
    {
        (A, R, bool) Eval(MValue ma, R state);
        MValue Lift((A, R, bool) value);
        MValue Lift(Func<R, (A, R, bool)> f);
        MValue Bottom { get; }
    }

    public interface WriterMonadValue<MValue, W, A>
    {
        (A, W, bool) Eval(MValue ma, W state);
        MValue Lift((A, W, bool) value);
        MValue Lift(Func<W, (A, W, bool)> f);
        MValue Bottom { get; }
    }

    public interface StateMonadValue<MValue, S, A>
    {
        (A, S, bool) Eval(MValue ma, S state);
        MValue Lift((A, S, bool) value);
        MValue Lift(Func<S, (A, S, bool)> f);
        MValue Bottom { get; }
    }
}
