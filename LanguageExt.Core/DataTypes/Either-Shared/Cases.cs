namespace LanguageExt
{
    /// <summary>
    /// Returned by the `This` property in all `Either*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public interface EitherCase<L, R>
    { }

    /// <summary>
    /// Left case
    /// 
    /// Returned by the `This` property in all `Either*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public sealed class LeftCase<L, R> : EitherCase<L, R>
    {
        public readonly L Value;

        internal LeftCase(L value) => 
            Value = value;

        public void Deconstruct(out L Value) => 
            Value = this.Value;

        public static implicit operator L(LeftCase<L, R> ma) => 
            ma.Value;

        public static implicit operator LeftCase<L, R>(L value) => 
            new LeftCase<L, R>(value);

        public static implicit operator LeftCase<L, R>(EitherLeft<L> ma) => 
            ma.Value;

        internal static EitherCase<L, R> New(L value) => 
            new LeftCase<L, R>(value);
    }

    /// <summary>
    /// Right case
    /// 
    /// Returned by the `This` property in all `Either*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public sealed class RightCase<L, R> : EitherCase<L, R>
    {
        public readonly R Value;

        internal RightCase(R value) => 
            Value = value;

        public void Deconstruct(out R Value) => 
            Value = this.Value;

        public static implicit operator R(RightCase<L, R> ma) => 
            ma.Value;

        public static implicit operator RightCase<L, R>(R value) => 
            new RightCase<L, R>(value);

        public static implicit operator RightCase<L, R>(EitherRight<R> ma) => 
            ma.Value;

        internal static EitherCase<L, R> New(R value) => 
            new RightCase<L, R>(value);
    }
}
