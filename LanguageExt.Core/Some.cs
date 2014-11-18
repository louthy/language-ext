using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public struct Some<T>
    {
        public T Value;

        public Some(T value)
        {
            if (value == null)
            {
                throw new ValueIsNullException("Value is null when expecting Some(x)");
            }

            if (value is IOptionalValue && ((IOptionalValue)value).IsNone)
            {
                throw new ValueIsNoneException("Value is None when expecting Some(x)");
            }

            Value = value;
        }

        public static implicit operator Some<T>(Option<T> value) => 
            match(
                value,
                Some: v => new Some<T>(v),
                None: () => raise<Some<T>>(new ValueIsNoneException("Value is None when expecting Some(x)"))
            );

        public static implicit operator Some<T>(T value) => new Some<T>(value);
        public static implicit operator T(Some<T> value) => value.Value;

        public override string ToString() =>
            Value.ToString();

        public override int GetHashCode() =>
            Value.GetHashCode();

        public override bool Equals(object obj) =>
            Value.Equals(obj);
    }
}
