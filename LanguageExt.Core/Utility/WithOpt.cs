using System;

namespace LanguageExt
{
    /// <summary>
    /// Do not use.  This is for the LanguageExt.CodeGen usage only
    /// </summary>
    public struct WithOpt<A>
    {
        readonly bool IsNone;
        readonly A Value;

        public WithOpt(A value)
        {
            IsNone = ReferenceEquals(value, null);
            Value = value;
        }

        public A IfNone(A defaultValue) =>
            IsNone
                ? defaultValue
                : Value;

        public static implicit operator WithOpt<A>(OptionNone _) =>
            new WithOpt<A>(default);

        public static implicit operator WithOpt<A>(A value) =>
            new WithOpt<A>(value);
    }
}
