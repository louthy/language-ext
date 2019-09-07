using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public delegate ReaderResult<A> Reader<Env, A>(Env env);

    public struct ReaderResult<A>
    {
        public static readonly ReaderResult<A> Bottom = new ReaderResult<A>();

        enum State
        {
            Bottom,
            Top
        }

        internal readonly A Value;
        readonly Option<Error> Error;
        State Init;

        ReaderResult(A value, Option<Error> error)
        {
            Value = value;
            Error = error;
            Init = State.Top;
        }

        internal Error ErrorInt => Init == State.Bottom
            ? Common.Error.Bottom
            : (Error)Error;

        public bool IsFaulted =>
            Init == State.Bottom || Error.IsSome;

        public static ReaderResult<A> New(A value) => 
            new ReaderResult<A>(value, None);

        public static ReaderResult<A> New(Error error) =>
            new ReaderResult<A>(default, Some(error));

        internal static ReaderResult<A> New(Option<Error> error) =>
            new ReaderResult<A>(default, error);

        public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
            IsFaulted
                ? Fail(ErrorInt)
                : Succ(Value);

        public A IfFail(A value) =>
            IsFaulted
                ? value
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
    }
}
