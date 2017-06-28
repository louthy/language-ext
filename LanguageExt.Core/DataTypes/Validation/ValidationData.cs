using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.DataTypes.Serialisation
{
    public class ValidationData<MonoidFail, FAIL, SUCCESS> : IEquatable<ValidationData<MonoidFail, FAIL, SUCCESS>>
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        public readonly bool IsBottom;
        public readonly SUCCESS Success;
        public readonly FAIL Fail;

        public ValidationData(bool isBottom, SUCCESS success, FAIL fail)
        {
            IsBottom = isBottom;
            Success = success;
            Fail = fail;
        }

        public override int GetHashCode() =>
            IsBottom ? -1
          : !default(MonoidFail).Equals(default(MonoidFail).Empty(), Fail) ? 0
          : Success?.GetHashCode() ?? 0;

        public static bool operator ==(ValidationData<MonoidFail, FAIL, SUCCESS> x, ValidationData<MonoidFail, FAIL, SUCCESS> y) =>
            x.Equals(y);

        public static bool operator !=(ValidationData<MonoidFail, FAIL, SUCCESS> x, ValidationData<MonoidFail, FAIL, SUCCESS> y) =>
            !(x == y);

        public bool Equals(ValidationData<MonoidFail, FAIL, SUCCESS> other) =>
            !ReferenceEquals(other, null) &&
            IsBottom == other.IsBottom &&
            default(MonoidFail).Equals(Fail, other.Fail) &&
            default(EqDefault<SUCCESS>).Equals(Success, other.Success);

        public override bool Equals(object obj) =>
            obj is EitherData<FAIL, SUCCESS> && Equals((EitherData<FAIL, SUCCESS>)obj);

        public override string ToString() =>
            IsBottom
                ? "Bottom"
                : default(MonoidFail).Equals(default(MonoidFail).Empty(), Fail)
                    ? Prelude.isnull(Fail)
                        ? "Left(null)"
                        : $"Left({Fail})"
                    : Prelude.isnull(Success)
                        ? "Right(null)"
                        : $"Right({Success})";
    }
}
