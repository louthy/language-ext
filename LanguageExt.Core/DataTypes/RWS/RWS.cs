using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public delegate RWSResult<MonoidW, R, W, S, A> RWS<MonoidW, R, W, S, A>(R env, S state)
        where MonoidW : struct, Monoid<W>;

    public struct RWSState<W, S>
    {
        public static readonly RWSState<W, S> Bottom = new RWSState<W, S>();

        enum Status
        {
            Bottom,
            Top
        }

        public readonly W Output;
        public readonly S State;
        internal readonly Option<Error> Error;
        Status Init;

        internal Error ErrorInt => Init == Status.Bottom
            ? Common.Error.Bottom
            : (Error)Error;

        public bool IsFaulted =>
            Init == Status.Bottom || Error.IsSome;

        internal RWSState(W output, S state, Option<Error> error)
        {
            Output = output;
            State = state;
            Error = error;
            Init = Status.Top;
        }
    }

    public struct RWSResult<MonoidW, R, W, S, A> : IEquatable<RWSResult<MonoidW, R, W, S, A>>, IEquatable<A>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly RWSResult<MonoidW, R, W, S, A> Bottom = new RWSResult<MonoidW, R, W, S, A>();

        enum Status
        {
            Bottom,
            Top
        }

        internal readonly A Value;
        public readonly W Output;
        public readonly S State;
        internal readonly Option<Error> Error;
        Status Init;

        RWSResult(W output, S state, A value, Option<Error> error)
        {
            Value = value;
            Output = output;
            State = state;
            Error = error;
            Init = Status.Top;
        }

        internal Error ErrorInt => Init == Status.Bottom || Error.IsNone
            ? Common.Error.Bottom
            : (Error)Error;

        public bool IsFaulted =>
            Init == Status.Bottom || Error.IsSome;

        public static RWSResult<MonoidW, R, W, S, A> New(S state, A value) =>
            new RWSResult<MonoidW, R, W, S, A>(default(MonoidW).Empty(), state, value, None);

        public static RWSResult<MonoidW, R, W, S, A> New(W output, S state, A value) =>
            new RWSResult<MonoidW, R, W, S, A>(output, state, value, None);

        public static RWSResult<MonoidW, R, W, S, A> New(S state, Error error) =>
            new RWSResult<MonoidW, R, W, S, A>(default(MonoidW).Empty(), state, default, Some(error));

        public static RWSResult<MonoidW, R, W, S, A> New(W output, S state, Error error) =>
            new RWSResult<MonoidW, R, W, S, A>(output, state, default, Some(error));

        internal static RWSResult<MonoidW, R, W, S, A> New(S state, Option<Error> error) =>
            new RWSResult<MonoidW, R, W, S, A>(default(MonoidW).Empty(), state, default, error);

        internal static RWSResult<MonoidW, R, W, S, A> New(W output, S state, Option<Error> error) =>
            new RWSResult<MonoidW, R, W, S, A>(output, state, default, error);

        public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
            IsFaulted
                ? Fail(ErrorInt)
                : Succ(Value);

        public Unit Match(Action<A> Succ, Action<Error> Fail)
        {
            if (IsFaulted)
            {
                Fail(ErrorInt);
            }
            else
            {
                Succ(Value);
            }
            return default;
        }

        public A IfFail(A value) =>
            IsFaulted
                ? value
                : Value;

        public A IfFailThrow() =>
            IsFaulted
                ? throw new InnerException(ErrorInt.ToException())
                : Value;

        public Option<A> ToOption() =>
            IsFaulted
                ? None
                : Some(Value);

        public OptionUnsafe<A> ToOptionUnsafe() =>
            IsFaulted
                ? None
                : SomeUnsafe(Value);

        public OptionAsync<A> ToOptionAsync() =>
            IsFaulted
                ? OptionAsync<A>.None
                : SomeAsync(Value);

        public Seq<A> ToSeq() =>
            IsFaulted
                ? Empty
                : Seq1(Value);

        public Lst<A> ToList() =>
            IsFaulted
                ? Empty
                : List(Value);

        public Either<Error, A> ToEither() =>
            IsFaulted
                ? Left<Error, A>(ErrorInt)
                : Right<Error, A>(Value);

        public Either<L, A> ToEither<L>(Func<Error, L> Left) =>
            IsFaulted
                ? Left<L, A>(Left(ErrorInt))
                : Right<L, A>(Value);

        public EitherUnsafe<Error, A> ToEitherUnsafe() =>
            IsFaulted
                ? LeftUnsafe<Error, A>(ErrorInt)
                : RightUnsafe<Error, A>(Value);

        public EitherUnsafe<L, A> ToEitherUnsafe<L>(Func<Error, L> Left) =>
            IsFaulted
                ? LeftUnsafe<L, A>(Left(ErrorInt))
                : RightUnsafe<L, A>(Value);

        public EitherAsync<Error, A> ToEitherAsync() =>
            IsFaulted
                ? LeftAsync<Error, A>(ErrorInt)
                : RightAsync<Error, A>(Value);

        public EitherAsync<L, A> ToEitherAsync<L>(Func<Error, L> Left) =>
            IsFaulted
                ? LeftAsync<L, A>(Left(ErrorInt))
                : RightAsync<L, A>(Value);

        public Try<A> ToTry() =>
            IsFaulted
                ? Try<A>((ErrorInt).ToException())
                : Try(Value);

        public TryAsync<A> ToTryAsync() =>
            IsFaulted
                ? TryAsync<A>((ErrorInt).ToException())
                : TryAsync(Value);

        public Reader<R, A> ToReader()
        {
            var value = Value;
            var error = Error;
            if (IsFaulted)
            {
                return (R _) => ReaderResult<A>.New(error);
            }
            else
            {
                return (R _) => ReaderResult<A>.New(value);
            }
        }

        public Writer<MonoidW, W, A> ToWriter()
        {
            var value = Value;
            var output = Output;
            var isFaulted = IsFaulted;
            return () => (value, output, isFaulted);
        }

        public State<S, A> ToState()
        {
            var value = Value;
            var state = State;
            var isFaulted = IsFaulted;
            return _ => (value, state, isFaulted);
        }

        internal RWSResult<MonoidW, R, W, S, B> SetValue<B>(B value) =>
            new RWSResult<MonoidW, R, W, S, B>(Output, State, value, Error);

        internal RWSState<W, S> ToRwsState() =>
            Init == Status.Bottom
                ? RWSState<W, S>.Bottom
                : new RWSState<W, S>(Output, State, Error);

        public override bool Equals(object obj) =>
            obj is RWSResult<MonoidW, R, W, S, A> res && Equals(res);

        public override int GetHashCode() =>
            IsFaulted ? 0 : Value?.GetHashCode() ?? 0;

        public bool Equals(RWSResult<MonoidW, R, W, S, A> right) =>
            IsFaulted == right.IsFaulted &&
            (IsFaulted || default(EqDefault<A>).Equals(Value, right.Value));

        public bool Equals(A right) =>
            !IsFaulted && default(EqDefault<A>).Equals(Value, right);

        public static bool operator ==(RWSResult<MonoidW, R, W, S, A> left, A right) =>
            left.Equals(right);

        public static bool operator !=(RWSResult<MonoidW, R, W, S, A> left, A right) =>
            !(left == right);

        public static bool operator ==(RWSResult<MonoidW, R, W, S, A> left, RWSResult<MonoidW, R, W, S, A> right) =>
            left.Equals(right);

        public static bool operator !=(RWSResult<MonoidW, R, W, S, A> left, RWSResult<MonoidW, R, W, S, A> right) =>
            !(left == right);

        public void Deconstruct(out A Value, out W Output, out S State, out Option<Error> Error)
        {
            Value = this.Value;
            Output = this.Output;
            State = this.State;
            Error = IsFaulted ? default : ErrorInt;
        }
    }
}
