using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    public interface IReturn
    {
        bool HasValue
        {
            get;
        }
    }

    public struct Return<T> : IReturn
    {
        public static readonly Return<T> None;

        public readonly T Value;

        public bool HasValue
        {
            get;
            private set;
        }

        internal Return(bool hasValue, T value)
        {
            HasValue = notnull(value) && hasValue;
            Value = value;
        }

        public static implicit operator Return<T>(T value) =>
            new Return<T>(true, value);

        public static implicit operator T(Return<T> value) =>
            value.Value;

        public static implicit operator Return<T>(NoReturn noreply) =>
            None;
    }

    public static class Return
    {
        public static NoReturn None =>
            NoReturn.Default;

        public static Return<T> Value<T>(T value) =>
            new Return<T>(true, value);
    }

    public class NoReturn : IReturn
    {
        public static readonly NoReturn Default = new NoReturn();

        public bool HasValue => 
            false;

        NoReturn()
        {
        }
    }
}
