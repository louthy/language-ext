using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public struct Some<T>
    {
        readonly T value;
        readonly bool initialised;

        public Some()
        {
            initialised = false;
            value = default(T);
            throw new SomeNotInitialisedException(typeof(T));
        }

        public Some(T value)
        {
            if (value == null)
            {
                throw new ValueIsNullException("Value is null when expecting Some(x)");
            }
            this.value = value;
            initialised = true;
        }

        public T Value => 
            CheckInitialised(value);

        private U CheckInitialised<U>(U value) =>
            initialised
                ? value
                : raise<U>( new SomeNotInitialisedException(typeof(T)) );

        public static implicit operator Option<T>(Some<T> value) =>
            Option<T>.Some(value.Value);

        public static implicit operator Some<T>(T value) => 
            new Some<T>(value);

        public static implicit operator T(Some<T> value) => 
            value.Value;

        public override string ToString() =>
            Value.ToString();

        public override int GetHashCode() =>
            Value.GetHashCode();

        public override bool Equals(object obj) =>
            Value.Equals(obj);
    }
}
