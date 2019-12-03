namespace LanguageExt
{
    public interface OptionCase<A>
    { }

    public class Some<A> : OptionCase<A>
    {
        readonly A Value;
        internal Some(A value) => Value = value;
        public void Deconstruct(out A Value) => Value = this.Value;
        public static implicit operator A(Some<A> ma) => ma.Value;
        public static implicit operator Some<A>(A value) => new Some<A>(value);
    }

    public class None<A> : OptionCase<A>
    {
        public static OptionCase<A> Default = new None<A>();
        internal None() { }
    }
}
