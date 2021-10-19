using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public delegate ReaderResult<A> Reader<Env, A>(Env env);

    public struct ReaderResult<A> : IEquatable<ReaderResult<A>>, IEquatable<A>
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

        internal Error ErrorInt => Init == State.Bottom || Error.IsNone
            ? Common.Errors.Bottom
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
                ? ErrorInt.ToException().Rethrow<A>()
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

        public override bool Equals(object obj) =>
            obj is ReaderResult<A> res && Equals(res);

        public override int GetHashCode() =>
            IsFaulted ? 0 : Value?.GetHashCode() ?? 0;

        public bool Equals(ReaderResult<A> right) =>
            IsFaulted == right.IsFaulted &&
            (IsFaulted || default(EqDefault<A>).Equals(Value, right.Value));

        public bool Equals(A right) =>
            !IsFaulted && default(EqDefault<A>).Equals(Value, right);

        public static bool operator ==(ReaderResult<A> left, A right) =>
            left.Equals(right);

        public static bool operator !=(ReaderResult<A> left, A right) =>
            !(left == right);

        public static bool operator ==(ReaderResult<A> left, ReaderResult<A> right) =>
            left.Equals(right);

        public static bool operator !=(ReaderResult<A> left, ReaderResult<A> right) =>
            !(left == right);

        public void Deconstruct(out A Value, out Option<Error> Error)
        {
            Value = this.Value;
            Error = IsFaulted ? default : ErrorInt;
        }
    }
}
