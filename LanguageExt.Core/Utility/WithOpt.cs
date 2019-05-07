using System;

namespace LanguageExt
{
    /// <summary>
    /// Do not use.  This is for the LanguageExt.CodeGen usage only
    /// </summary>
    public struct WithOpt<A>
    {
        readonly bool IsSome;
        readonly A Value;

        public WithOpt(A value)
        {
            IsSome = !ReferenceEquals(value, null);
            Value = value;
        }

        public A IfNone(A defaultValue) =>
            IsSome
                ? Value
                : defaultValue;

        public static implicit operator WithOpt<A>(OptionNone _) =>
            default;

        public static implicit operator WithOpt<A>(A value) =>
            new WithOpt<A>(value);
    }
}
