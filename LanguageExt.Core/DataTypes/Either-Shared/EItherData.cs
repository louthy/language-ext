using LanguageExt.ClassInstances;
using System;

namespace LanguageExt.DataTypes.Serialisation
{
    public static class EitherData
    {
        public static EitherData<L, R> Right<L, R>(R rightValue) => 
            new EitherData<L, R>(EitherStatus.IsRight, rightValue, default(L));

        public static EitherData<L, R> Left<L, R>(L leftValue) =>
            new EitherData<L, R>(EitherStatus.IsLeft, default(R), leftValue);

        public static EitherData<L, R> Bottom<L, R>() =>
            EitherData<L, R>.Bottom;
    }

    public class EitherData<L, R> : IEquatable<EitherData<L, R>>
    {
        public static EitherData<L, R> Bottom = new EitherData<L, R>(EitherStatus.IsBottom, default(R), default(L));

        public readonly EitherStatus State;
        public readonly R Right;
        public readonly L Left;

        public EitherData(EitherStatus state, R right, L left)
        {
            State = state;
            Right = right;
            Left = left;
        }

        public override int GetHashCode() =>
            State == EitherStatus.IsBottom ? -1
          : State == EitherStatus.IsRight  ? Right?.GetHashCode() ?? 0
          : Left?.GetHashCode() ?? 0;

        public static bool operator ==(EitherData<L, R> x, EitherData<L, R> y) =>
            x.Equals(y);

        public static bool operator !=(EitherData<L, R> x, EitherData<L, R> y) =>
            !(x == y);

        public bool Equals(EitherData<L, R> other) =>
            !ReferenceEquals(other, null) && 
            State == other.State &&
            default(EqDefault<L>).Equals(Left, other.Left) &&
            default(EqDefault<R>).Equals(Right, other.Right);

        public override bool Equals(object obj) =>
            obj is EitherData<L, R> && Equals((EitherData<L, R>)obj);

        public override string ToString() =>
            State == EitherStatus.IsBottom
                ? "Bottom"
                : State == EitherStatus.IsRight
                    ? Prelude.isnull(Right)
                        ? "Right(null)"
                        : $"Right({Right})"
                    : Prelude.isnull(Left)
                        ? "Left(null)"
                        : $"Left({Left})";
    }
}
