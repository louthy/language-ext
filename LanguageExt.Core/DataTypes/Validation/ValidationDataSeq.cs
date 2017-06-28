using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.DataTypes.Serialisation
{
    public class ValidationData<FAIL, SUCCESS> : IEquatable<ValidationData<FAIL, SUCCESS>>
    {
        public readonly bool IsBottom;
        public readonly SUCCESS Success;
        public readonly Seq<FAIL> Fail;

        public ValidationData(bool isBottom, SUCCESS success, Seq<FAIL> fail)
        {
            IsBottom = isBottom;
            Success = success;
            Fail = fail;
        }

        public override int GetHashCode() =>
            IsBottom ? -1
          : Fail?.IsEmpty ?? false ? 0
          : Success?.GetHashCode() ?? 0;

        public static bool operator ==(ValidationData<FAIL, SUCCESS> x, ValidationData<FAIL, SUCCESS> y) =>
            x.Equals(y);

        public static bool operator !=(ValidationData<FAIL, SUCCESS> x, ValidationData<FAIL, SUCCESS> y) =>
            !(x == y);

        public bool Equals(ValidationData<FAIL, SUCCESS> other) =>
            !ReferenceEquals(other, null) &&
            IsBottom == other.IsBottom &&
            (Fail?.IsEmpty ?? false) == (other.Fail?.IsEmpty ?? false) &&
            default(EqDefault<SUCCESS>).Equals(Success, other.Success);

        public override bool Equals(object obj) =>
            obj is EitherData<FAIL, SUCCESS> && Equals((EitherData<FAIL, SUCCESS>)obj);

        public override string ToString() =>
            IsBottom
                ? "Bottom"
                : Fail?.IsEmpty ?? false
                    ? Prelude.isnull(Fail)
                        ? "Left(null)"
                        : $"Left({Fail})"
                    : Prelude.isnull(Success)
                        ? "Right(null)"
                        : $"Right({Success})";
    }
}
