namespace LanguageExt
{
    /// <summary>
    /// Returned by the `This` property in all `Validation*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public interface ValidationCase<FAIL, SUCCESS>
    { }

    /// <summary>
    /// Left case
    /// 
    /// Returned by the `This` property in all `Validation*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public sealed class FailCase<FAIL, SUCCESS> : ValidationCase<FAIL, SUCCESS>
    {
        public readonly FAIL Value;

        internal FailCase(FAIL value) => 
            Value = value;

        public void Deconstruct(out FAIL Value) => 
            Value = this.Value;

        internal static ValidationCase<FAIL, SUCCESS> New(FAIL value) => 
            new FailCase<FAIL, SUCCESS>(value);
    }

    /// <summary>
    /// Right case
    /// 
    /// Returned by the `This` property in all `Validation*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public sealed class SuccCase<FAIL, SUCCESS> : ValidationCase<FAIL, SUCCESS>
    {
        public readonly SUCCESS Value;

        internal SuccCase(SUCCESS value) => 
            Value = value;

        public void Deconstruct(out SUCCESS Value) => 
            Value = this.Value;

        internal static ValidationCase<FAIL, SUCCESS> New(SUCCESS value) => 
            new SuccCase<FAIL, SUCCESS>(value);
    }
}
