namespace LanguageExt
{
    /// <summary>
    /// Returned by the `This` property in all `Option*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public interface OptionCase<A>
    { }

    /// <summary>
    /// Some case
    /// 
    /// Returned by the `This` property in all `Option*` types.  It facilitates
    /// the C# pattern-maching functionality.
    /// </summary>
    public sealed class SomeCase<A> : OptionCase<A>
    {
        public readonly A Value;

        internal SomeCase(A value) => 
            Value = value;

        public void Deconstruct(out A Value) => 
            Value = this.Value;

        internal static OptionCase<A> New(A value) => 
            new SomeCase<A>(value);

        public static implicit operator A(SomeCase<A> ma) => 
            ma.Value;
        
        public static implicit operator SomeCase<A>(A value) => 
            new SomeCase<A>(value);
    }

    /// <summary>
    /// None case
    /// 
    /// Returned by the `This` property in all `Option*` types and `TryOption*` types.  
    /// It facilitates the C# pattern-maching functionality.
    /// </summary>
    public sealed class NoneCase<A> : OptionCase<A>, TryCase<A>
    {
        internal static NoneCase<A> Default = new NoneCase<A>();
        internal NoneCase() { }
    }
}
